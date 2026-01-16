<template>
  <q-header v-if="$q.screen.gt.xs" class="app-header bg-dark">
    <q-toolbar>
      <q-toolbar-title class="text-center text-subtitle1 text-bold">{{ title }}</q-toolbar-title>
    </q-toolbar>
  </q-header>
</template>

<script setup lang="ts">
import { computed } from "vue";

const title = computed(() => import.meta.env.VITE_APP_NAME);
</script>

<style scoped>
.app-header {
  height: 24px !important;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
