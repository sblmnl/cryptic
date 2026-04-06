<template>
  <q-page class="column q-pa-md">
    <div class="col row justify-center">
      <div class="col-lg-6 col-sm-9 col-xs-12">
        <CreateNoteForm @save-success="onSaveSuccess" @save-failed="onSaveFailed" />
      </div>
    </div>
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
import type { CodedError } from "@/shared/types/error";
import { Notify } from "quasar";
import { ref } from "vue";
import type { CreateNoteHttpResponse } from "../api/create-note";
import CreateNoteForm from "../components/CreateNoteForm.vue";
import NoteCreatedPopup from "../components/NoteCreatedPopup.vue";
import type { NoteId } from "../note";

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
