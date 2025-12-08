<template>
  <v-form ref="formRef" v-model="valid" @submit="submit">
    <v-container>
      <v-row>
        <v-textarea
          v-model="formData.content"
          :rules="[rules.content.required, rules.content.minLength, rules.content.maxLength]"
          class="pb-2"
          name="content"
          placeholder="Enter your note here..."
          variant="outlined"
          maxlength="10000"
          rows="10"
          clearable
          counter
          persistent-counter
        ></v-textarea>
      </v-row>
      <v-row v-if="$vuetify.display.xlAndUp" class="mt-5 gap-3">
        <v-select
          v-model="formData.deleteAfter"
          :items="deleteAfterOptions"
          name="delete-after"
          label="Delete after"
          variant="outlined"
          item-title="text"
        ></v-select>
        <v-text-field
          v-model="formData.encryptionPassword"
          :append-inner-icon="encryptionPasswordVisible ? 'mdi-eye-off' : 'mdi-eye'"
          :type="encryptionPasswordVisible ? 'text' : 'password'"
          name="encryption-password"
          label="Encryption Password"
          variant="outlined"
          @click:append-inner="encryptionPasswordVisible = !encryptionPasswordVisible"
        />
        <v-text-field
          v-model="formData.accessPassword"
          :append-inner-icon="accessPasswordVisisble ? 'mdi-eye-off' : 'mdi-eye'"
          :type="accessPasswordVisisble ? 'text' : 'password'"
          name="access-password"
          label="Access Password"
          variant="outlined"
          @click:append-inner="accessPasswordVisisble = !accessPasswordVisisble"
        />
      </v-row>
      <v-row v-if="$vuetify.display.lgAndDown" class="mt-5">
        <v-select
          v-model="formData.deleteAfter"
          :items="deleteAfterOptions"
          name="delete-after"
          item-title="text"
          label="Delete after"
          variant="outlined"
        ></v-select>
      </v-row>
      <v-row v-if="$vuetify.display.lgAndDown" class="mt-5">
        <v-text-field
          v-model="formData.encryptionPassword"
          :append-inner-icon="encryptionPasswordVisible ? 'mdi-eye-off' : 'mdi-eye'"
          :type="encryptionPasswordVisible ? 'text' : 'password'"
          name="encryption-password"
          label="Encryption Password"
          variant="outlined"
          @click:append-inner="encryptionPasswordVisible = !encryptionPasswordVisible"
        />
      </v-row>
      <v-row v-if="$vuetify.display.lgAndDown" class="mt-5">
        <v-text-field
          v-model="formData.accessPassword"
          :append-inner-icon="accessPasswordVisisble ? 'mdi-eye-off' : 'mdi-eye'"
          :type="accessPasswordVisisble ? 'text' : 'password'"
          name="access-password"
          label="Access Password"
          variant="outlined"
          @click:append-inner="accessPasswordVisisble = !accessPasswordVisisble"
        />
      </v-row>
      <v-row v-if="$vuetify.display.mdAndUp" justify="space-between">
        <v-btn ref="cancelBtnRef" :disabled="loading" color="darkgray" flat @click="cancel">Cancel</v-btn>
        <v-btn ref="submitBtnRef" :disabled="!valid" :loading="loading" type="submit" color="success" flat
          >Create</v-btn
        >
      </v-row>
      <v-row v-if="$vuetify.display.smAndDown" class="mb-5">
        <v-btn ref="submitBtnRef" :disabled="!valid" :loading="loading" type="submit" color="success" width="100%" flat
          >Create</v-btn
        >
      </v-row>
      <v-row v-if="$vuetify.display.smAndDown">
        <v-btn ref="cancelBtnRef" :disabled="loading" color="darkgray" width="100%" flat @click="cancel">Cancel</v-btn>
      </v-row>
    </v-container>
  </v-form>
</template>

<script setup lang="ts">
import { DeleteAfter } from "@/core/enums/common";
import type { CodedError } from "@/core/errors";
import type { FailedHttpResponseBody, OkHttpResponseBody } from "@/core/http";
import { uint8ArrayToBase64Replacer } from "@/core/json";
import { createNote, type CreateNoteHttpRequest, type CreateNoteHttpResponse } from "@/modules/notes/models";
import { getAppRoute } from "@/router";
import axios from "axios";
import { reactive, ref, type Ref } from "vue";
import type { VBtn, VForm } from "vuetify/components";

const emit = defineEmits<{
  saveSuccess: [CreateNoteHttpResponse];
  saveFailed: [CodedError[]];
}>();

function getDefaultFormData() {
  return {
    content: "",
    encryptionPassword: "",
    accessPassword: "",
    deleteAfter: DeleteAfter.Viewing,
  };
}

const valid = ref();
const encryptionPasswordVisible = ref(false);
const accessPasswordVisisble = ref(false);
const loading = ref(false);

const formData = reactive(getDefaultFormData());
const formRef: Ref<VForm | null> = ref(null);

const deleteAfterOptions = DeleteAfter.listAll();

const rules = {
  content: {
    required: (value: string) => !!value || "Note cannot be empty",
    minLength: (value: string) => value.length >= 3 || "Note must contain at least 3 characters",
    maxLength: (value: string) => value.length <= 10_000 || "Note cannot exceed 10,000 characters",
  },
};

function resetForm() {
  Object.assign(formData, getDefaultFormData());
  formRef.value?.resetValidation();
}

async function submit(e: Event) {
  e.preventDefault();

  loading.value = true;

  const note = await createNote(formData.content, formData.encryptionPassword);
  const requestBody: CreateNoteHttpRequest = {
    content: note.content,
    deleteAfter: formData.deleteAfter.value,
    password: formData.accessPassword.length > 0 ? formData.accessPassword : undefined,
    clientMetadata: JSON.stringify(note.clientMetdata, uint8ArrayToBase64Replacer),
  };

  try {
    const response = await axios.post<OkHttpResponseBody<CreateNoteHttpResponse>>(
      getAppRoute("/api/notes"),
      requestBody,
    );

    loading.value = false;
    resetForm();

    emit("saveSuccess", response.data.data);
  } catch (err) {
    loading.value = false;

    if (!axios.isAxiosError<FailedHttpResponseBody>(err)) {
      throw err;
    }

    emit("saveFailed", err.response?.data.errors ?? []);
  }
}

function cancel() {
  resetForm();
}
</script>
