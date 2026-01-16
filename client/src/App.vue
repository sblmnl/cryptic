<template>
  <DefaultAppLayout v-if="$route.meta.layout === AppLayout.Default">
    <router-view />
  </DefaultAppLayout>
  <router-view v-if="$route.meta.layout === AppLayout.None" />
</template>

<script setup lang="ts">
import { AppLayout, DefaultAppLayout } from "@/layouts";
</script>
