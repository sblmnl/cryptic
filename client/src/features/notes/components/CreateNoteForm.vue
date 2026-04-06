<template>
  <q-form ref="formRef" v-model="isValid" class="column full-height full-width">
    <div class="col-xs col-md-6">
      <q-input
        v-model="formData.content"
        :rules="[
          (val) => (val && val.length >= 3) || 'Note must contain at least 3 characters',
          (val) => val.length <= 10_000 || 'Note cannot exceed 10,000 characters',
        ]"
        class="full-height"
        input-class="full-height"
        placeholder="Enter your note here..."
        type="textarea"
        maxlength="10000"
        outlined
        bottom-slots
        clearable
        counter
        autofocus
      />
    </div>

    <div v-if="$q.screen.gt.sm" class="row q-col-gutter-md q-mt-sm">
      <div class="col-4">
        <q-select
          v-model="formData.deleteAfter"
          :options="DeleteAfter.listAll()"
          option-value="value"
          option-label="text"
          name="delete-after"
          label="Delete After"
          class="col-4"
          outlined
        />
      </div>
      <div class="col-4">
        <q-input
          v-model="formData.accessPassword"
          :type="hideAccessPwd ? 'password' : 'text'"
          name="access-password"
          label="Access Password"
          hint="Protects access to the note and its metadata. This password is sent to the server."
          class="col-4"
          outlined
        >
          <template #append>
            <q-icon
              :name="hideAccessPwd ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hideAccessPwd = !hideAccessPwd"
            />
          </template>
        </q-input>
      </div>
      <div class="col-4">
        <q-input
          v-model="formData.encryptionPassword"
          :type="hideEncryptionPwd ? 'password' : 'text'"
          name="encryption-password"
          label="Encryption Password"
          hint="Used for client-side encryption of the note content and is never sent to the server."
          outlined
        >
          <template #append>
            <q-icon
              :name="hideEncryptionPwd ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hideEncryptionPwd = !hideEncryptionPwd"
            />
          </template>
        </q-input>
      </div>
    </div>
    <div v-else class="column q-col-gutter-sm q-mt-sm">
      <div class="col-4">
        <q-select
          v-model="formData.deleteAfter"
          :options="DeleteAfter.listAll()"
          option-value="value"
          option-label="text"
          name="delete-after"
          label="Delete After"
          outlined
        />
      </div>
      <div class="col-4">
        <q-input
          v-model="formData.accessPassword"
          :type="hideAccessPwd ? 'password' : 'text'"
          name="access-password"
          label="Access Password"
          outlined
        >
          <template #append>
            <q-icon
              :name="hideAccessPwd ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hideAccessPwd = !hideAccessPwd"
            />
          </template>
        </q-input>
      </div>
      <div class="col-4">
        <q-input
          v-model="formData.encryptionPassword"
          :type="hideEncryptionPwd ? 'password' : 'text'"
          name="encryption-password"
          label="Encryption Password"
          outlined
        >
          <template #append>
            <q-icon
              :name="hideEncryptionPwd ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hideEncryptionPwd = !hideEncryptionPwd"
            />
          </template>
        </q-input>
      </div>
    </div>

    <div v-if="$q.screen.gt.sm" class="row justify-between q-mt-md">
      <q-btn label="Cancel" flat @click="onCancelBtnClick" />
      <q-btn
        :disable="!isValid"
        :loading="noteCreationInProgress"
        label="Create"
        color="positive"
        style="width: 96px"
        unelevated
        @click="onCreateBtnClick"
      />
    </div>
    <div v-else class="q-mt-lg">
      <div>
        <q-btn
          :disable="!isValid"
          :loading="noteCreationInProgress"
          label="Create"
          color="positive"
          class="full-width"
          unelevated
          @click="onCreateBtnClick"
        />
      </div>
      <div class="q-mt-xs">
        <q-btn label="Cancel" class="full-width" unelevated @click="onCancelBtnClick" />
      </div>
    </div>
  </q-form>
</template>

<script setup lang="ts">
import {
  sendCreateNoteRequest,
  type CreateNoteHttpRequest,
  type CreateNoteHttpResponse,
} from "@/features/notes/api/create-note";
import { createNote, type Note } from "@/features/notes/note";
import type { FailedHttpResponseBody } from "@/shared/api/http-response-body";
import DeleteAfter from "@/shared/types/delete-after";
import type { CodedError } from "@/shared/types/error";
import { uint8ArrayToBase64Replacer } from "@/shared/util/json";
import { isAxiosError } from "axios";
import type { QForm } from "quasar";
import { nextTick, reactive, ref, watch } from "vue";

const emit = defineEmits<{
  saveSuccess: [CreateNoteHttpResponse];
  saveFailed: [CodedError[]];
}>();

interface CreateNoteFormData {
  content: string;
  deleteAfter: DeleteAfter;
  accessPassword: string;
  encryptionPassword: string;
}

function getDefaultFormData(): CreateNoteFormData {
  return {
    content: "",
    deleteAfter: DeleteAfter.Viewing,
    accessPassword: "",
    encryptionPassword: "",
  };
}

const formRef = ref<QForm>();

const isValid = ref(false);
const formData = reactive(getDefaultFormData());

const hideAccessPwd = ref(true);
const hideEncryptionPwd = ref(true);

const noteCreationInProgress = ref(false);

async function validateForm() {
  if (!formRef.value) {
    return;
  }

  isValid.value = await formRef.value.validate();
}

watch(formData, validateForm, { immediate: true });

async function resetForm() {
  Object.assign(formData, getDefaultFormData());

  await nextTick(() => {
    formRef.value?.resetValidation();
  });
}

function onCancelBtnClick() {
  return resetForm();
}

async function onCreateBtnClick() {
  if (!isValid.value) {
    return;
  }

  noteCreationInProgress.value = true;

  const note: Note = await createNote(formData.content, formData.encryptionPassword);

  const reqBody: CreateNoteHttpRequest = {
    content: note.content,
    deleteAfter: formData.deleteAfter.value,
    password: formData.accessPassword.length > 0 ? formData.accessPassword : undefined,
    clientMetadata: JSON.stringify(note.clientMetadata, uint8ArrayToBase64Replacer),
  };

  try {
    const data = await sendCreateNoteRequest(reqBody);
    await resetForm();
    emit("saveSuccess", data);
  } catch (err) {
    emit("saveFailed", isAxiosError<FailedHttpResponseBody>(err) ? (err.response?.data.errors ?? []) : []);
  }

  noteCreationInProgress.value = false;
}
</script>

<style scoped>
:deep(.q-textarea .q-field__control) {
  height: 100% !important;
}
</style>
