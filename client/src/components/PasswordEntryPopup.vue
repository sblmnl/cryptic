<template>
  <q-dialog v-model="visible" :persistent="persistent">
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
          <div :class="['text-h6']">{{ title }}</div>
        </div>
        <q-btn v-if="!persistent" icon="close" round unelevated @click="closePopup" />
      </q-card-section>

      <q-card-section class="column q-col-gutter-md q-pt-sm q-px-md">
        <div>{{ text }}</div>
        <q-input v-model="passwordEntry" :type="hidePassword ? 'password' : 'text'" :label="fieldLabel" outlined>
          <template #append>
            <q-icon
              :name="hidePassword ? 'visibility' : 'visibility_off'"
              class="cursor-pointer"
              @click="hidePassword = !hidePassword"
            />
          </template>
        </q-input>
      </q-card-section>

      <q-card-actions class="q-px-md q-pb-md">
        <div class="row reverse justify-between full-width">
          <q-btn :label="submitBtnText" color="positive" unelevated @click="onSubmitBtnClick" />
          <q-btn v-if="!persistent" label="Cancel" color="grey" flat @click="closePopup" />
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

function closePopup() {
  passwordEntry.value = "";
  hidePassword.value = false;
  visible.value = false;
}

function onSubmitBtnClick() {
  emit("submit", { password: passwordEntry.value });
  closePopup();
}
</script>
