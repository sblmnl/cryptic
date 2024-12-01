<template>
  <v-navigation-drawer>
    <v-list>
      <router-link to="/" class="nav-logo">
        <v-list-item prepend-avatar="/logo.svg" title="Cryptic"></v-list-item>
      </router-link>
    </v-list>
    
    <v-divider></v-divider>

    <v-list density="compact" nav>
      <v-list-item prepend-icon="mdi-pencil-outline" title="Notes" value="notes" to="/notes"></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<style scoped>
.nav-logo {
  text-decoration: none;
  color: inherit;
}
</style>
