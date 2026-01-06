<template>
  <v-dialog v-model="visible" :persistent="persistent" width="600px">
    <v-card>
      <v-card-title class="d-flex justify-content-between">
        {{ title ?? "" }}
        <v-spacer></v-spacer>
        <v-btn v-if="showCloseButton" icon variant="flat">
          <v-icon @click="closePopup">mdi-close</v-icon>
        </v-btn>
      </v-card-title>

      <v-card-text>
        {{ text ?? "" }}
        <v-text-field
          v-model="passwordEntry"
          :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
          :type="showPassword ? 'text' : 'password'"
          :label="fieldLabel"
          :class="text ? ['mt-5'] : []"
          variant="outlined"
          @click:append-inner="showPassword = !showPassword"
        />
      </v-card-text>

      <v-card-actions>
        <v-btn v-if="showCancelButton" color="darkgray" @click="closePopup">Cancel</v-btn>
        <v-btn color="success" @click="onSubmitBtnClicked">{{ submitButtonText }}</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, type Ref } from "vue";

const visible = defineModel<boolean>({ default: false, required: true });

defineProps({
  title: { type: String, required: false },
  text: { type: String, required: false },
  fieldLabel: { type: String, required: false },
  submitButtonText: { type: String, required: false, default: "Submit" },
  showCancelButton: { type: Boolean, required: false, default: true },
  showCloseButton: { type: Boolean, required: false, default: true },
  persistent: { type: Boolean, required: false, default: false },
});

export interface PasswordEntryPopupSubmitEvent {
  password: string | null;
}

const emit = defineEmits<{ submit: [PasswordEntryPopupSubmitEvent] }>();

const passwordEntry: Ref<string | null> = ref(null);
const showPassword = ref(false);

function closePopup() {
  passwordEntry.value = null;
  showPassword.value = false;
  visible.value = false;
}

function onSubmitBtnClicked() {
  emit("submit", { password: passwordEntry.value });
  closePopup();
}
</script>
