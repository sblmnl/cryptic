<template>
  <q-page class="column q-pa-md">
    <div class="col row justify-center">
      <div class="col-lg-6 col-sm-9 col-xs-12">
        <div class="column full-height full-width">
          <div class="col-xs col-md-6">
            <q-input
              :model-value="note.content"
              rows="16"
              type="textarea"
              class="full-height"
              input-class="full-height"
              outlined
              readonly
            >
              <template #append>
                <q-icon v-if="note.content.length > 0" name="eva-copy-outline" />
              </template>
            </q-input>
          </div>
          <div class="row items-center justify-end q-mt-sm">
            <q-btn to="/notes" label="Create a new note" color="positive" unelevated />
          </div>
        </div>
      </div>
    </div>
    <PasswordEntryPopup
      v-model="showAccessPwdPopup"
      title="Enter Access Password"
      field-label="Access Password"
      persistent
      @submit="onAccessPwdPopupSubmit"
    />
    <PasswordEntryPopup
      v-model="showEncryptionPwdPopup"
      title="Enter Encryption Password"
      field-label="Encryption Password"
      submit-btn-text="Decrypt"
      persistent
      @submit="onEncryptionPwdPopupSubmit"
    />
  </q-page>
</template>

<script setup lang="ts">
import { api } from "@/boot/axios";
import PasswordEntryPopup, { type PasswordEntryPopupSubmitEvent } from "@/components/PasswordEntryPopup.vue";
import type { FailedHttpResponseBody, OkHttpResponseBody } from "@/lib/models/api";
import type { ReadNoteHttpResponse } from "@/lib/models/notes/api/read-note";
import { decryptNote, type Note, type NoteClientMetadata } from "@/lib/models/notes/note";
import { type AxiosError, isAxiosError } from "axios";
import { Loading, Notify } from "quasar";
import { validate as validateUuid } from "uuid";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();

const noteId = computed(() => route.params.noteId as string);

const note = reactive<Note>({
  id: "",
  content: "",
});

const accessPwd = ref<string>();

const showAccessPwdPopup = ref(false);
const showEncryptionPwdPopup = ref(false);

function onAccessPwdPopupSubmit(e: PasswordEntryPopupSubmitEvent) {
  accessPwd.value = e.password;
  return readNote();
}

async function onEncryptionPwdPopupSubmit(e: PasswordEntryPopupSubmitEvent) {
  if (e.password.length === 0) {
    return;
  }

  try {
    note.content = await decryptNote(note, e.password);
    Notify.create({ type: "positive", message: "Note decrypted!" });
  } catch {
    Notify.create({ type: "negative", message: "Incorrect encryption password!" });
    showEncryptionPwdPopup.value = true;
  }
}

function handleUnauthorizedError() {
  if (accessPwd.value) {
    Notify.create({ type: "negative", message: "Incorrect access password!" });
  }

  showAccessPwdPopup.value = true;
}

function handleAxiosError(err: AxiosError<FailedHttpResponseBody>) {
  if (err.status === 404) {
    Notify.create({ type: "negative", message: "Note not found!" });
    return;
  }

  if (err.status === 401) {
    handleUnauthorizedError();
    return;
  }

  const data = err.response?.data ?? { errors: [] };

  Notify.create({
    type: "negative",
    message:
      data.errors.length > 0
        ? data.errors.map((x) => x.message).join("; ")
        : "An error occurred while attempting to read the note!",
  });
}

function detectAndHandleEncryptedNote(clientMetadataString: string | null) {
  if (clientMetadataString == null) {
    return;
  }

  try {
    note.clientMetadata = JSON.parse(clientMetadataString) as NoteClientMetadata;
  } catch {
    Notify.create({ type: "negative", message: "Unable to read the note's metadata!" });
    return;
  }

  if (note.clientMetadata.clientName !== import.meta.env.VITE_CLIENT_NAME) {
    Notify.create({
      type: "warning",
      message: "This note was created using a different client and may not be properly read.",
    });
  }

  showEncryptionPwdPopup.value = true;
}

async function readNote() {
  try {
    Loading.show();

    const res = await api.post<OkHttpResponseBody<ReadNoteHttpResponse>>(`/notes/${noteId.value}/read`, null, {
      params: {
        password: accessPwd.value,
      },
    });

    Loading.hide();

    if (res.data.data.destroyed) {
      Notify.create({ type: "info", message: "Note destroyed!" });
    }

    Object.assign(note, {
      id: res.data.data.noteId,
      content: res.data.data.content,
    });

    detectAndHandleEncryptedNote(res.data.data.clientMetadata);
  } catch (err) {
    Loading.hide();

    if (isAxiosError<FailedHttpResponseBody>(err)) {
      handleAxiosError(err);
      return;
    }

    Notify.create({ type: "negative", message: "An error occurred while attempting to read the note!" });
  }
}

onMounted(async () => {
  if (!validateUuid(noteId.value)) {
    Notify.create({ type: "negative", message: "Invalid note ID!" });
    return;
  }

  await readNote();
});
</script>

<style scoped>
:deep(.q-textarea .q-field__control) {
  height: 100% !important;
}
</style>
