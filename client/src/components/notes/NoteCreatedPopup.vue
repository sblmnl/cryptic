<template>
  <q-dialog v-model="visible">
    <q-card style="width: 600px">
      <q-card-section
        :class="[
          'row',
          'no-wrap',
          $q.screen.width < 379 ? 'items-start' : 'items-center',
          'justify-between',
          'q-pb-none',
        ]"
      >
        <div>
          <div :class="['text-h6']">Your note has been created!</div>
        </div>
        <q-btn icon="close" round unelevated @click="close" />
      </q-card-section>

      <q-card-section class="column q-col-gutter-md q-pt-sm q-px-md">
        <q-input :model-value="noteUrl" type="text" label="Note URL" outlined readonly>
          <template #append>
            <q-icon :name="copyNoteUrlIcon" class="cursor-pointer" @click="onCopyNoteUrlBtnClick" />
          </template>
        </q-input>
        <q-input
          :model-value="controlToken"
          :type="hideControlToken ? 'password' : 'text'"
          label="Control Token"
          outlined
          readonly
        >
          <template #append>
            <q-icon
              :name="hideControlToken ? 'visibility' : 'visibility_off'"
              class="cursor-pointer"
              @click="hideControlToken = !hideControlToken"
            />
            <q-icon :name="copyControlTokenIcon" class="cursor-pointer" @click="onCopyControlTokenBtnClick" />
          </template>
        </q-input>
      </q-card-section>

      <q-card-actions align="between" class="q-px-md q-pb-md">
        <q-btn color="negative" style="width: 96px" flat @click="onDestroyBtnClick">
          <q-spinner v-if="noteDestructionInProgress" />
          <span v-else>Destroy</span>
        </q-btn>
        <q-btn label="Ok" color="positive" unelevated @click="onOkBtnClick" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import type { CodedError } from "@/lib/common";
import type { FailedHttpResponseBody } from "@/lib/models/api";
import { sleep } from "@/lib/util/time";
import { appBaseUrl } from "@/router";
import { api } from "@/wrappers/axios";
import { isAxiosError } from "axios";
import { computed, ref } from "vue";

const visible = defineModel({ type: Boolean, default: false });

const props = defineProps<{
  noteId: string;
  controlToken: string;
}>();

const emit = defineEmits<{
  destroySuccess: [];
  destroyFailed: [CodedError[]];
}>();

const noteUrl = computed(() => `${appBaseUrl}/notes/${props.noteId}`);

const hideControlToken = ref(true);
const copyNoteUrlIcon = ref<"content_copy" | "check">("content_copy");
const copyControlTokenIcon = ref<"content_copy" | "check">("content_copy");

const noteDestructionInProgress = ref(false);

function close() {
  visible.value = false;
}

async function onCopyNoteUrlBtnClick() {
  if (copyNoteUrlIcon.value === "check") {
    return;
  }

  await navigator.clipboard.writeText(noteUrl.value);
  copyNoteUrlIcon.value = "check";
  await sleep(500);
  copyNoteUrlIcon.value = "content_copy";
}

async function onCopyControlTokenBtnClick() {
  if (copyControlTokenIcon.value === "check") {
    return;
  }

  await navigator.clipboard.writeText(props.controlToken);
  copyControlTokenIcon.value = "check";
  await sleep(500);
  copyControlTokenIcon.value = "content_copy";
}

function onOkBtnClick() {
  close();
}

async function onDestroyBtnClick() {
  noteDestructionInProgress.value = true;

  try {
    await api.delete(`/notes/${props.noteId}`, {
      params: {
        controlToken: props.controlToken,
      },
    });

    emit("destroySuccess");
  } catch (err) {
    emit("destroyFailed", isAxiosError<FailedHttpResponseBody>(err) ? (err.response?.data.errors ?? []) : []);
  }

  noteDestructionInProgress.value = false;
  close();
}
</script>
