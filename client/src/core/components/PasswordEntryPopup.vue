<template>
  <v-dialog v-model="visible" width="600px">
    <v-card>
      <v-card-title class="d-flex justify-content-between">
        {{ title }}
        <v-spacer></v-spacer>
        <v-btn icon variant="flat">
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
        <v-btn color="darkgray" @click="closePopup">Cancel</v-btn>
        <v-btn color="success" @click="onSubmitBtnClicked">Submit</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, type Ref } from "vue";

const visible = defineModel<boolean>({ default: false, required: true });

defineProps<{
  title: string;
  text?: string;
  fieldLabel: string;
}>();

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
