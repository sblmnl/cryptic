<template>
  <q-header v-if="$q.screen.gt.sm" class="app-header bg-transparent">
    <q-toolbar class="q-pt-md">
      <div class="row full-width">
        <div class="col-4">
          <router-link to="/">
            <q-chip :label="title" class="text-bold no-shadow q-pa-md" clickable />
          </router-link>
        </div>

        <div class="row col-4 justify-center">
          <router-link v-for="link in navLinks" :key="link.route" :to="link.route">
            <q-chip
              :icon="link.icon"
              :label="link.text"
              :color="isLinkActive(link.route) ? (Dark.isActive ? 'grey' : 'dark') : ''"
              :text-color="isLinkActive(link.route) ? (Dark.isActive ? 'black' : 'white') : ''"
              class="text-bold q-pa-md q-mr-sm no-shadow"
              clickable
            />
          </router-link>
        </div>

        <div class="row col-4 justify-end">
          <q-chip class="no-shadow q-pa-md" clickable @click="isDarkModeActive = !isDarkModeActive">
            <q-icon :name="isDarkModeActive ? 'eva-sun-outline' : 'eva-moon-outline'" size="xs" />
          </q-chip>
        </div>
      </div>
    </q-toolbar>
  </q-header>
  <q-header v-else class="app-header bg-transparent">
    <q-toolbar class="q-pt-md q-pl-xs">
      <q-btn
        :text-color="Dark.isActive ? 'white' : 'black'"
        icon="eva-menu-outline"
        round
        flat
        dense
        @click="drawer = !drawer"
      />
    </q-toolbar>
  </q-header>
  <q-drawer v-model="drawer" overlay bordered>
    <q-scroll-area class="fit">
      <q-list>
        <q-item>
          <q-item-section>
            <div class="row justify-between">
              <span class="text-h6">{{ title }}</span>
              <q-btn
                :icon="isDarkModeActive ? 'eva-sun-outline' : 'eva-moon-outline'"
                flat
                round
                dense
                @click="isDarkModeActive = !isDarkModeActive"
              />
            </div>
          </q-item-section>
        </q-item>

        <q-separator />

        <q-item v-for="link in navLinks" :key="link.route" v-ripple :to="link.route" exact clickable>
          <q-item-section avatar>
            <q-icon :name="link.icon" />
          </q-item-section>
          <q-item-section class="text-subtitle1">{{ link.text }}</q-item-section>
        </q-item>
      </q-list>
    </q-scroll-area>
  </q-drawer>
</template>

<script setup lang="ts">
import { Dark } from "quasar";
import { ref, watch } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();

const title = import.meta.env.VITE_APP_NAME;
const drawer = ref(false);

const navLinks = [
  { icon: "eva-file-text-outline", text: "Notes", route: "/notes", rootAlias: true },
  { icon: "eva-heart-outline", text: "Contribute", route: "/contribute" },
];

const isDarkModeActive = ref(Dark.isActive);

watch(
  isDarkModeActive,
  (val) => {
    Dark.set(val);
    localStorage.setItem("darkMode", val.toString());
  },
  { immediate: true },
);

function isLinkActive(routePath: string) {
  const matches = navLinks.filter((x) => x.route === routePath);

  if (matches.length === 0) {
    return false;
  }

  const match = matches[0]!;

  if (match.rootAlias && route.path === "/") {
    return true;
  }

  return route.path.toLowerCase().startsWith(routePath.toLowerCase());
}
</script>

<style scoped>
.app-header {
  height: 32px !important;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
