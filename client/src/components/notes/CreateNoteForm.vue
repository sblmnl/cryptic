<template>
  <q-form ref="formRef" v-model="isValid">
    <div class="row justify-center">
      <div class="col-xl-6 col-sm-9 col-xs-12">
        <q-input
          v-model="formData.content"
          :rules="[
            (val) => (val && val.length >= 3) || 'Note must contain at least 3 characters',
            (val) => val.length <= 10_000 || 'Note cannot exceed 10,000 characters',
          ]"
          placeholder="Enter your note here..."
          rows="16"
          type="textarea"
          maxlength="10000"
          outlined
          bottom-slots
          clearable
          counter
          lazy-rules
        />

        <div v-if="$q.screen.gt.sm" class="row q-mt-sm q-col-gutter-md">
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
              hint="Protects access to the note and its metadata. This password is sent to the server."
              outlined
            >
              <template #append>
                <q-icon
                  :name="hideAccessPwd ? 'visibility' : 'visibility_off'"
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
                  :name="hideEncryptionPwd ? 'visibility' : 'visibility_off'"
                  class="cursor-pointer"
                  @click="hideEncryptionPwd = !hideEncryptionPwd"
                />
              </template>
            </q-input>
          </div>
        </div>
        <div v-else class="column q-mt-sm q-col-gutter-sm">
          <div>
            <q-select
              v-model="formData.deleteAfter"
              :options="DeleteAfter.listAll()"
              option-value="value"
              option-label="text"
              label="Delete After"
              outlined
            />
          </div>
          <div>
            <q-input
              v-model="formData.accessPassword"
              :type="hideAccessPwd ? 'password' : 'text'"
              name="access-password"
              label="Access Password"
              outlined
            >
              <template #append>
                <q-icon
                  :name="hideAccessPwd ? 'visibility' : 'visibility_off'"
                  class="cursor-pointer"
                  @click="hideAccessPwd = !hideAccessPwd"
                />
              </template>
            </q-input>
          </div>
          <div>
            <q-input
              v-model="formData.encryptionPassword"
              :type="hideEncryptionPwd ? 'password' : 'text'"
              name="encryption-password"
              label="Encryption Password"
              outlined
            >
              <template #append>
                <q-icon
                  :name="hideEncryptionPwd ? 'visibility' : 'visibility_off'"
                  class="cursor-pointer"
                  @click="hideEncryptionPwd = !hideEncryptionPwd"
                />
              </template>
            </q-input>
          </div>
        </div>

        <div v-if="$q.screen.gt.sm" class="row justify-between q-mt-md">
          <q-btn label="Cancel" color="grey" flat @click="onCancelBtnClick" />
          <q-btn color="positive" style="width: 96px" unelevated @click="onCreateBtnClick">
            <q-spinner v-if="noteCreationInProgress" />
            <span v-else>Create</span>
          </q-btn>
        </div>
        <div v-else class="q-mt-lg">
          <div>
            <q-btn color="positive" class="full-width" unelevated @click="onCreateBtnClick">
              <q-spinner v-if="noteCreationInProgress" />
              <span v-else>Create</span>
            </q-btn>
          </div>
          <div class="q-mt-xs">
            <q-btn label="Cancel" color="grey-8" class="full-width" unelevated @click="onCancelBtnClick" />
          </div>
        </div>
      </div>
    </div>
  </q-form>
</template>

<script setup lang="ts">
import { DeleteAfter } from "@/lib/common";
import type { CodedError } from "@/lib/common/errors";
import type { FailedHttpResponseBody, OkHttpResponseBody } from "@/lib/models/api";
import type { CreateNoteHttpRequest, CreateNoteHttpResponse } from "@/lib/models/notes/api/create-note";
import { createNote, type Note } from "@/lib/models/notes/note";
import { uint8ArrayToBase64Replacer } from "@/lib/util/json";
import { api } from "@/wrappers/axios";
import { isAxiosError } from "axios";
import type { QForm } from "quasar";
import { reactive, ref } from "vue";

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

const isValid = ref(true);
const formData = reactive(getDefaultFormData());

const hideAccessPwd = ref(true);
const hideEncryptionPwd = ref(true);

const noteCreationInProgress = ref(false);

function resetForm() {
  Object.assign(formData, getDefaultFormData());
  formRef.value?.resetValidation();
}

function onCancelBtnClick() {
  resetForm();
}

async function onCreateBtnClick() {
  noteCreationInProgress.value = true;

  const note: Note = await createNote(formData.content, formData.encryptionPassword);

  const reqBody: CreateNoteHttpRequest = {
    content: note.content,
    deleteAfter: formData.deleteAfter.value,
    password: formData.accessPassword.length > 0 ? formData.accessPassword : undefined,
    clientMetadata: JSON.stringify(note.clientMetadata, uint8ArrayToBase64Replacer),
  };

  try {
    const res = await api.post<OkHttpResponseBody<CreateNoteHttpResponse>>("/notes", reqBody);
    resetForm();
    emit("saveSuccess", res.data.data);
  } catch (err) {
    emit("saveFailed", isAxiosError<FailedHttpResponseBody>(err) ? (err.response?.data.errors ?? []) : []);
  }

  noteCreationInProgress.value = false;
}
</script>
