<template>
  <v-dialog v-model="visible" width="600px">
    <v-card>
      <v-card-title class="d-flex justify-content-between">
        Your note has been created!
        <v-spacer></v-spacer>
        <v-btn icon variant="flat">
          <v-icon @click="visible = false">mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-card-text>
        <v-text-field
          :model-value="noteUrl"
          name="note-url"
          class="mb-2"
          label="Note URL"
          variant="outlined"
          readonly
          persistent-hint
        >
          <template v-slot:append-inner>
            <v-icon :disabled="copyNoteUrlIcon !== 'mdi-clipboard'" @click="onCopyNoteUrlBtnClicked">{{
              copyNoteUrlIcon
            }}</v-icon>
          </template>
        </v-text-field>
        <v-text-field
          :model-value="controlToken"
          :type="showControlToken ? 'text' : 'password'"
          name="control-token"
          label="Control Token"
          variant="outlined"
          readonly
        >
          <template v-slot:append-inner>
            <v-icon
              class="me-1"
              :disabled="copyNoteUrlIcon !== 'mdi-clipboard'"
              @click="onCopyControlTokenBtnClicked"
              >{{ copyControlTokenIcon }}</v-icon
            >
            <v-icon @click="showControlToken = !showControlToken">{{
              showControlToken ? "mdi-eye-off" : "mdi-eye"
            }}</v-icon>
          </template>
        </v-text-field>
      </v-card-text>

      <v-card-actions>
        <v-btn color="danger" :loading="destroyingNote" @click="onDestroyNoteBtnClicked">Destroy</v-btn>
        <v-btn color="darkgray" @click="visible = false">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { sleep } from "@/core/utilities";
import type { NoteId } from "@/modules/notes/models";
import { appBaseUrl, getAppRoute } from "@/router";
import axios from "axios";
import { computed, ref, type Ref } from "vue";

const visible = defineModel<boolean>({ default: false, required: true });

const props = defineProps<{
  noteId: NoteId;
  controlToken: string;
}>();

const noteUrl = computed(() => appBaseUrl + getAppRoute(`/notes/${props.noteId}`));
const showControlToken = ref(false);
const destroyingNote = ref(false);
const copyNoteUrlIcon: Ref<"mdi-clipboard" | "mdi-check"> = ref("mdi-clipboard");
const copyControlTokenIcon: Ref<"mdi-clipboard" | "mdi-check"> = ref("mdi-clipboard");

async function onCopyNoteUrlBtnClicked() {
  await navigator.clipboard.writeText(noteUrl.value);
  copyNoteUrlIcon.value = "mdi-check";
  await sleep(500);
  copyNoteUrlIcon.value = "mdi-clipboard";
}

async function onCopyControlTokenBtnClicked() {
  await navigator.clipboard.writeText(props.controlToken);
  copyControlTokenIcon.value = "mdi-check";
  await sleep(500);
  copyControlTokenIcon.value = "mdi-clipboard";
}

async function onDestroyNoteBtnClicked() {
  destroyingNote.value = true;

  try {
    const response = await axios.delete(getAppRoute(`/api/notes/${props.noteId}?controlToken=${props.controlToken}`));
    console.log(response);
  } catch (err) {
    console.log(err);
  }

  destroyingNote.value = false;
}
</script>
