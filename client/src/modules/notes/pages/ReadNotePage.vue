<template>
  <v-container fluid class="justify-content-center align-items-center">
    <v-row justify="center">
      <v-col md="8" lg="6">
        <v-container>
          <v-row>
            <v-textarea
              :model-value="noteContent"
              class="pb-2"
              name="content"
              variant="outlined"
              maxlength="10000"
              rows="10"
              readonly
            ></v-textarea>
          </v-row>
        </v-container>
      </v-col>
    </v-row>
    <PasswordEntryPopup
      v-model="accessPasswordPopupVisible"
      title="Enter Access Password"
      text="This note is password-protected, please enter the access password."
      field-label="Access Password"
      @submit="onAccessPasswordEntryPopupSubmitted"
    />
    <PasswordEntryPopup
      v-model="encryptionPasswordPopupVisible"
      title="Enter Encryption Password"
      text="This note is encrypted, please enter the encryption password."
      field-label="Encryption Password"
      @submit="onEncryptionPasswordEntryPopupSubmitted"
    />
  </v-container>
  <v-snackbar v-model="toastVisible" :color="toastColor" timeout="3000" variant="flat">
    <div class="text-center">
      {{ toastMessage }}
    </div>
  </v-snackbar>
  <v-overlay :model-value="loading" class="align-center justify-center">
    <v-progress-circular color="white" indeterminate size="64"></v-progress-circular>
  </v-overlay>
</template>

<script setup lang="ts">
import PasswordEntryPopup, { type PasswordEntryPopupSubmitEvent } from "@/core/components/PasswordEntryPopup.vue";
import type { OkHttpResponseBody } from "@/core/http";
import { decryptNote, type Note, type NoteClientMetadata, type ReadNoteHttpResponse } from "@/modules/notes/models";
import { getAppRoute } from "@/router";
import axios, { AxiosError } from "axios";
import { validate as validateUuid } from "uuid";
import { onMounted, ref, type Ref } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();

const loading = ref(false);

const toastVisible = ref(false);
const toastColor = ref<"success" | "warning" | "danger" | "darkgray">("danger");
const toastMessage = ref("Note not found!");

const noteContent: Ref<string | null> = ref(null);
const noteClientMetadata: Ref<NoteClientMetadata | null> = ref(null);

const accessPasswordPopupVisible = ref(false);
const accessPassword: Ref<string | null> = ref(null);

const encryptionPasswordPopupVisible = ref(false);

function showToast(color: "success" | "warning" | "danger" | "darkgray", message: string) {
  toastColor.value = color;
  toastMessage.value = message;
  toastVisible.value = true;
}

function showAccessPasswordPrompt() {
  accessPasswordPopupVisible.value = true;
}

function showEncryptionPasswordPrompt() {
  encryptionPasswordPopupVisible.value = true;
}

async function onEncryptionPasswordEntryPopupSubmitted(e: PasswordEntryPopupSubmitEvent) {
  if (e.password == null || e.password.length === 0) {
    return;
  }

  const note: Note = { content: noteContent.value!, clientMetadata: noteClientMetadata.value ?? undefined };

  try {
    noteContent.value = await decryptNote(note, e.password);
  } catch (err) {
    console.log(err);
    showToast("danger", "Incorrect encryption password!");
    showEncryptionPasswordPrompt();
  }
}

async function onAccessPasswordEntryPopupSubmitted(e: PasswordEntryPopupSubmitEvent) {
  accessPassword.value = e.password;
  await readNote();
}

function handleUnauthorizedError() {
  if (accessPassword.value) {
    showToast("danger", "Incorrect access password!");
  }

  showAccessPasswordPrompt();
}

function handleAxiosError(err: AxiosError) {
  if (err.status === 404) {
    showToast("danger", "Note not found!");
    return;
  }

  if (err.status === 401) {
    handleUnauthorizedError();
    return;
  }
}

async function detectAndHandleEncryptedNote(clientMetadataString: string | null) {
  if (clientMetadataString == null) {
    return;
  }

  try {
    noteClientMetadata.value = JSON.parse(clientMetadataString) as NoteClientMetadata;
  } catch {
    showToast("danger", "Unable to read the note's metadata!");
    return;
  }

  if (noteClientMetadata.value.clientName !== import.meta.env.VITE_CLIENT_NAME) {
    showToast("warning", "This note was created using a different client and may not be properly read.");
  }

  showEncryptionPasswordPrompt();
}

async function readNote() {
  const noteId = route.params.noteId as string;

  if (!validateUuid(noteId)) {
    toastMessage.value = "Invalid note ID!";
    toastVisible.value = true;
    return;
  }

  try {
    loading.value = true;

    const noteRoute = getAppRoute(`/api/notes/${noteId}/read`);
    const response = await axios.post<OkHttpResponseBody<ReadNoteHttpResponse>>(noteRoute, null, {
      params: {
        password: accessPassword.value,
      },
    });

    loading.value = false;

    noteContent.value = response.data.data.content;

    if (response.data.data.destroyed) {
      showToast("darkgray", "The note has been destroyed!");
    }

    await detectAndHandleEncryptedNote(response.data.data.clientMetadata);
  } catch (err) {
    loading.value = false;

    if (err instanceof AxiosError) {
      handleAxiosError(err);
      return;
    }

    showToast("danger", "An error has occurred while attempting to read the note!");
  }
}

onMounted(readNote);
</script>
