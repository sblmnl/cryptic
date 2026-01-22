<template>
  <q-dialog v-model="visible">
    <q-card style="width: 600px" class="no-shadow">
      <q-card-section
        :class="[
          'row',
          'no-wrap',
          $q.screen.width < 379 ? 'items-start' : 'items-center',
          'justify-between',
          'q-pb-none',
        ]"
      >
        <div class="text-h6">Your note has been created!</div>
        <q-space />
        <q-btn v-close-popup icon="eva-close-outline" flat round dense />
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
              :name="hideControlToken ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hideControlToken = !hideControlToken"
            />
            <q-icon :name="copyControlTokenIcon" class="cursor-pointer" @click="onCopyControlTokenBtnClick" />
          </template>
        </q-input>
      </q-card-section>

      <q-card-actions align="between" class="q-px-md q-pb-md">
        <q-btn
          :loading="noteDestructionInProgress"
          color="negative"
          label="Destroy"
          style="width: 96px"
          flat
          @click="onDestroyBtnClick"
        />
        <q-btn label="Ok" color="positive" unelevated @click="onOkBtnClick" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { api } from "@/boot/axios";
import type { CodedError } from "@/lib/common";
import type { FailedHttpResponseBody } from "@/lib/models/api";
import { sleep } from "@/lib/util/time";
import { appBaseUrl } from "@/router";
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
const copyNoteUrlIcon = ref<"eva-copy-outline" | "eva-checkmark-outline">("eva-copy-outline");
const copyControlTokenIcon = ref<"eva-copy-outline" | "eva-checkmark-outline">("eva-copy-outline");

const noteDestructionInProgress = ref(false);

function close() {
  visible.value = false;
}

async function onCopyNoteUrlBtnClick() {
  if (copyNoteUrlIcon.value === "eva-checkmark-outline") {
    return;
  }

  await navigator.clipboard.writeText(noteUrl.value);
  copyNoteUrlIcon.value = "eva-checkmark-outline";
  await sleep(500);
  copyNoteUrlIcon.value = "eva-copy-outline";
}

async function onCopyControlTokenBtnClick() {
  if (copyControlTokenIcon.value === "eva-checkmark-outline") {
    return;
  }

  await navigator.clipboard.writeText(props.controlToken);
  copyControlTokenIcon.value = "eva-checkmark-outline";
  await sleep(500);
  copyControlTokenIcon.value = "eva-copy-outline";
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
