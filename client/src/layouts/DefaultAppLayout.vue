<template>
  <q-layout view="hHh lpr fFf">
    <AppHeader />
    <q-page-container>
      <slot></slot>
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import AppHeader from "@/components/AppHeader.vue";
</script>
