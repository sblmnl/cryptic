<template>
  <q-dialog v-model="visible" :persistent="persistent">
    <q-card class="no-shadow" style="width: 600px">
      <q-card-section
        :class="[
          'row',
          'no-wrap',
          $q.screen.width < 379 ? 'items-start' : 'items-center',
          'justify-between',
          'q-pb-none',
        ]"
      >
        <div class="text-h6">{{ title }}</div>
        <q-space />
        <q-btn v-if="!persistent" icon="eva-close-outline" flat round dense @click="close" />
      </q-card-section>

      <q-card-section class="column q-col-gutter-md q-pt-sm q-px-md">
        <div>{{ text }}</div>
        <q-input
          v-model="passwordEntry"
          :type="hidePassword ? 'password' : 'text'"
          :label="fieldLabel"
          outlined
          autofocus
          @keyup.enter="submit"
        >
          <template #append>
            <q-icon
              :name="hidePassword ? 'eva-eye-outline' : 'eva-eye-off-outline'"
              class="cursor-pointer"
              @click="hidePassword = !hidePassword"
            />
          </template>
        </q-input>
      </q-card-section>

      <q-card-actions class="q-px-md q-pb-md">
        <div class="row reverse justify-between full-width">
          <q-btn
            :label="submitBtnText"
            :disable="!passwordEntry || passwordEntry.length === 0"
            color="positive"
            unelevated
            @click="submit"
          />
          <q-btn v-if="!persistent" label="Cancel" color="grey" flat @click="close" />
        </div>
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";

const visible = defineModel({ type: Boolean, default: false });

defineProps({
  title: { type: String, required: false, default: "Enter Password" },
  text: { type: String, required: false, default: "" },
  fieldLabel: { type: String, required: false, default: "Password" },
  submitBtnText: { type: String, required: false, default: "Submit" },
  persistent: { type: Boolean, required: false, default: false },
});

export interface PasswordEntryPopupSubmitEvent {
  password: string;
}

const emit = defineEmits<{ submit: [PasswordEntryPopupSubmitEvent] }>();

const passwordEntry = ref<string>("");
const hidePassword = ref(true);

function close() {
  passwordEntry.value = "";
  hidePassword.value = true;
  visible.value = false;
}

function submit() {
  if (!passwordEntry.value || passwordEntry.value.length === 0) {
    return;
  }

  emit("submit", { password: passwordEntry.value });
  close();
}
</script>
