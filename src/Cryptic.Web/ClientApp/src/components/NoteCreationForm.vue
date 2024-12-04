<template>
  <v-sheet class="mx-auto" :width="$vuetify.display.mobile ? '95%' : '50%'">
    <h1>Create a note</h1>
    <v-form fast-fail @submit.prevent>
      <v-textarea
        v-model="content"
        :rules="contentRules"
        placeholder="Write your note here..."
        :rows="$vuetify.display.mobile ? '15' : '20'"
        :no-resize="$vuetify.display.mobile"
        variant="outlined"
        class="my-2"
        clearable
        counter
        required
      ></v-textarea>

      <section>
        <section class="d-flex flex-column">
          <v-combobox
            v-model="deleteAfter"
            :items="deleteAfterOptions.map((x) => x.name)"
            label="Delete after"
            prepend-inner-icon="mdi-timer-outline"
            variant="outlined"
            class="my-2"
          ></v-combobox>

          <v-text-field
            v-model="password"
            label="Password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Enter your password"
            prepend-inner-icon="mdi-lock-outline"
            variant="outlined"
            class="my-2"
            counter
            clearable
          >
            <template v-slot:append-inner>
              <v-icon class="mx-2" @click="showPassword = !showPassword">
                {{ showPassword ? "mdi-eye-off" : "mdi-eye" }}
              </v-icon>
              <v-icon class="mx-2" @click="generatePassword()"
                >mdi-refresh</v-icon
              >
            </template>
          </v-text-field>
        </section>

        <section class="d-flex justify-space-between my-2">
          <v-btn
            type="button"
            color="green"
            prepend-icon="mdi-send"
            @click="submitForm()"
            >Create note</v-btn
          >
          <v-btn
            type="button"
            color="red"
            prepend-icon="mdi-broom"
            variant="outlined"
            @click="resetForm()"
            >Reset</v-btn
          >
        </section>
      </section>
    </v-form>
  </v-sheet>
</template>

<script>
import { DateTime, Duration } from "luxon";
import axios from "axios";
import passwordGenerator from "generate-password-browser";

export default {
  methods: {
    generatePassword() {
      this.password = passwordGenerator.generate({
        length: 16,
        numbers: true,
        symbols: true,
      });
    },
    resetForm() {
      this.content = "";
      this.password = "";
      this.deleteAfter = this.deleteAfterDefault;
    },
    async submitForm() {
      const deleteAfter = this.deleteAfterOptions.filter(
        (x) => x.name === this.deleteAfter,
      )[0];

      const res = await axios.post("/api/notes", {
        content: this.content,
        deleteAfterTime:
          deleteAfter.time === null
            ? null
            : DateTime.now().plus(deleteAfter.time),
        deleteOnReceipt: deleteAfter.reading,
        password: this.password === "" ? null : this.password,
      });

      console.log(JSON.stringify(res));
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
    deleteAfterOptions: [
      {
        name: "Reading",
        reading: true,
        time: null,
      },
      {
        name: "An hour",
        reading: false,
        time: Duration.fromObject({ hours: 1 }),
      },
      {
        name: "A day",
        reading: false,
        time: Duration.fromObject({ days: 1 }),
      },
      {
        name: "A week",
        reading: false,
        time: Duration.fromObject({ weeks: 1 }),
      },
    ],
    password: "",
    showPassword: false,
  }),
};
</script>
