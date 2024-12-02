<template>
  <v-sheet class="mx-auto" width="50%">
    <h1>Create a note</h1>
    <v-form fast-fail @submit.prevent>
      <v-textarea
        v-model="content"
        :rules="contentRules"
        placeholder="Write your note here..."
        rows="15"
        variant="outlined"
        auto-grow
        clearable
        counter
        required
      ></v-textarea>

      <section class="d-flex flex-column my-2">
        <v-combobox
          v-model="deleteAfter"
          :items="deleteAfterOptions"
          label="Delete after"
          prepend-inner-icon="mdi-timer-outline"
          variant="outlined"
        ></v-combobox>

        <v-text-field
          v-model="password"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Enter your password"
          prepend-inner-icon="mdi-lock-outline"
          variant="outlined"
        >
          <template v-slot:append-inner>
            <v-icon class="mx-2" @click="showPassword = !showPassword">
              {{ showPassword ? "mdi-eye-off" : "mdi-eye" }}
            </v-icon>
            <v-icon class="mx-2" @click="generatePassword()">mdi-refresh</v-icon>
            <v-icon v-if="password" class="mx-2" @click="password = ''">mdi-close-circle</v-icon>
          </template>
        </v-text-field>
      </section>

      <section class="d-flex justify-space-between my-2">
        <v-btn type="button" color="green" prepend-icon="mdi-send">Create note</v-btn>
        <v-btn type="button" color="red" prepend-icon="mdi-broom" variant="outlined" @click="resetForm()">Reset</v-btn>
      </section>
    </v-form>
  </v-sheet>
</template>

<script>
import passwordGenerator from "generate-password-browser";

export default {
  methods: {
    generatePassword() {
      this.password = passwordGenerator.generate({
        length: 16,
        numbers: true,
        symbols: true
      });
    },
    resetForm() {
      this.content = "";
      this.password = "";
      this.deleteAfter = this.deleteAfterDefault;
    },
  },
  data: () => ({
    content: "",
    contentRules: [
      (value) => {
        if (value) return true;

        return "Note cannot be empty!";
      },
      (value) => {
        if (value?.length <= 5_000) return true;

        return "Note cannot contain more than 5,000 characters!";
      },
    ],
    deleteAfterDefault: "Reading",
    deleteAfter: "Reading",
    deleteAfterOptions: ["Reading", "An hour", "A day", "A week"],
    password: "",
    showPassword: false,
  }),
};
</script>
