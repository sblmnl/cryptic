<template>
  <v-container fluid class="justify-content-center align-items-center">
    <v-row justify="center">
      <v-col md="8" lg="6">
        <CreateNoteForm @save-success="onCreateNoteSaveSuccess" @save-failed="onCreateNoteSaveFailed" />
      </v-col>
    </v-row>
    <NoteCreatedPopup
      v-model="noteCreatedPopupVisible"
      :note-id="createdNoteInfo.noteId"
      :control-token="createdNoteInfo.controlToken"
    />
  </v-container>
  <v-snackbar v-model="toastVisible" :color="toastColor" timeout="3000" variant="flat">
    <div class="text-center">
      {{ toastMessage }}
    </div>
  </v-snackbar>
</template>

<script setup lang="ts">
import type { CodedError } from "@/core/errors";
import CreateNoteForm from "@/modules/notes/components/CreateNoteForm.vue";
import NoteCreatedPopup from "@/modules/notes/components/NoteCreatedPopup.vue";
import type { CreateNoteHttpResponse } from "@/modules/notes/models";
import { ref, type Ref } from "vue";

const noteCreatedPopupVisible = ref(false);

const createdNoteInfo: Ref<CreateNoteHttpResponse> = ref({
  noteId: "",
  controlToken: "",
});

const toastVisible = ref(false);
const toastMessage = ref("Note created successfully!");
const toastColor: Ref<"success" | "danger"> = ref("success");

function onCreateNoteSaveSuccess(data: CreateNoteHttpResponse) {
  toastColor.value = "success";
  toastMessage.value = "Note created successfully!";
  toastVisible.value = true;
  noteCreatedPopupVisible.value = true;
  createdNoteInfo.value = data;
}

function onCreateNoteSaveFailed(errors: CodedError[]) {
  toastColor.value = "danger";
  toastMessage.value =
    errors.length > 0
      ? errors.map((x) => x.message).join("; ")
      : "An error occurred while attempting to create the note!";
  toastVisible.value = true;
}
</script>
