<template>
  <q-page class="column q-pa-md">
    <div class="col row justify-center">
      <div class="col-lg-6 col-sm-9 col-xs-12">
        <div>
          <span class="text-h5">Development</span>
          <q-separator class="q-my-sm" />
          <div class="text-body1 q-mb-sm">
            If you'd like to report bugs, request features, or otherwise contribute to development efforts, you can do
            so via the project's GitHub repository.
          </div>
          <q-btn
            :color="Dark.isActive ? 'dark' : 'grey'"
            :text-color="Dark.isActive ? 'white' : 'black'"
            :class="$q.screen.lt.md ? ['full-width'] : []"
            :href="githubRepoUrl"
            target="_blank"
            rel="noopener noreferrer"
            unelevated
          >
            <q-icon name="eva-github-outline" size="sm" />
            <span class="q-ml-sm">GitHub Repository</span>
            <q-icon name="eva-link-outline" class="q-ml-sm" size="xs" />
          </q-btn>
        </div>
        <div class="q-mt-lg">
          <span class="text-h5">Donations</span>
          <q-separator class="q-my-sm" />
          <div class="text-body1 q-mb-sm">
            If slinging code isn't exactly your thing and you'd still like to contribute, donations are always very much
            appreciated.
          </div>
          <div :class="$q.screen.lt.md ? ['column', 'q-gutter-y-sm'] : ['row', 'q-gutter-x-sm']">
            <q-btn
              v-if="kofiUrl"
              :color="Dark.isActive ? 'dark' : 'grey'"
              :text-color="Dark.isActive ? 'white' : 'black'"
              :href="kofiUrl"
              target="_blank"
              rel="noopener noreferrer"
              unelevated
            >
              <img src="/assets/img/logos/kofi.svg" height="18px" />
              <span class="q-ml-sm">Support me on Ko-fi</span>
              <q-icon name="eva-link-outline" class="q-ml-sm" size="xs" />
            </q-btn>
            <q-btn
              v-if="moneroAddress"
              :color="Dark.isActive ? 'dark' : 'grey'"
              :text-color="Dark.isActive ? 'white' : 'black'"
              unelevated
              @click="copyMoneroAddress"
            >
              <img src="/assets/img/logos/monero.svg" height="18px" />
              <span class="q-ml-sm">Donate Monero (XMR)</span>
              <q-icon :name="copyMoneroAddressIcon" class="q-ml-sm" size="xs" />
            </q-btn>
          </div>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { sleep } from "@/lib/util/time";
import { Dark } from "quasar";
import { ref } from "vue";

const githubRepoUrl = import.meta.env.VITE_GITHUB_REPO_URL;
const kofiUrl = import.meta.env.VITE_DONATE_KOFI_URL;
const moneroAddress = import.meta.env.VITE_DONATE_MONERO_ADDRESS;

const copyMoneroAddressIcon = ref<"eva-copy-outline" | "eva-checkmark-outline">("eva-copy-outline");

async function copyMoneroAddress() {
  await navigator.clipboard.writeText(moneroAddress);
  copyMoneroAddressIcon.value = "eva-checkmark-outline";
  await sleep(500);
  copyMoneroAddressIcon.value = "eva-copy-outline";
}
</script>
