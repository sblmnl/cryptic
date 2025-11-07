<template>
  <DefaultAppLayout v-if="$route.meta.layout === AppLayout.Default">
    <RouterView />
  </DefaultAppLayout>
  <RouterView v-if="$route.meta.layout === AppLayout.None" />
</template>

<script setup lang="ts">
import { AppLayout } from "@/core/enums/ui";
import DefaultAppLayout from "@/core/layouts/app/DefaultAppLayout.vue";
</script>
