<template>
  <q-page class="q-pa-md">
    <CreateNoteForm @save-success="onSaveSuccess" @save-failed="onSaveFailed" />
    <NoteCreatedPopup
      v-model="noteCreatedPopupVisible"
      :note-id="createdNoteId"
      :control-token="createdNoteControlToken"
      @destroy-success="onDestroySuccess"
      @destroy-failed="onDestroyFailed"
    />
  </q-page>
</template>

<script setup lang="ts">
import CreateNoteForm from "@/components/notes/CreateNoteForm.vue";
import NoteCreatedPopup from "@/components/notes/NoteCreatedPopup.vue";
import type { CodedError } from "@/lib/common";
import type { CreateNoteHttpResponse } from "@/lib/models/notes/api/create-note";
import type { NoteId } from "@/lib/models/notes/note";
import { Notify } from "quasar";
import { ref } from "vue";

const createdNoteId = ref<NoteId>("");
const createdNoteControlToken = ref("");
const noteCreatedPopupVisible = ref(false);

function onSaveSuccess(data: CreateNoteHttpResponse) {
  Notify.create({ type: "positive", message: "Note created successfully!" });

  createdNoteId.value = data.noteId;
  createdNoteControlToken.value = data.controlToken;
  noteCreatedPopupVisible.value = true;
}

function onSaveFailed(errors: CodedError[]) {
  Notify.create({
    type: "negative",
    message:
      errors.length > 0
        ? errors.map((x) => x.message).join("; ")
        : "An error occurred while attempting to create the note!",
  });
}

function onDestroySuccess() {
  Notify.create({ type: "positive", message: "Note destroyed!" });
}

function onDestroyFailed(errors: CodedError[]) {
  Notify.create({
    type: "negative",
    message:
      errors.length > 0
        ? errors.map((x) => x.message).join("; ")
        : "An error occurred while attempting to destroy the note!",
  });
}
</script>
