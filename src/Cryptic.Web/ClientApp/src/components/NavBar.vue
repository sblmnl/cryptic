<template>
  <v-app-bar-nav-icon
    v-if="$vuetify.display.mobile"
    @click="mobileDrawer = !mobileDrawer"
  ></v-app-bar-nav-icon>

  <!-- Desktop Nav -->
  <v-navigation-drawer v-if="!$vuetify.display.mobile" expand-on-hover rail>
    <v-list>
      <router-link to="/" class="nav-logo">
        <v-list-item prepend-avatar="/logo.svg" title="Cryptic"></v-list-item>
      </router-link>
    </v-list>

    <v-divider></v-divider>

    <v-list density="compact" nav>
      <v-list-item
        v-for="tab in tabs"
        :key="tab.key"
        :prepend-icon="tab.icon"
        :title="tab.title"
        :to="tab.to"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>

  <!-- Mobile Nav -->
  <v-navigation-drawer
    v-if="$vuetify.display.mobile"
    v-model="mobileDrawer"
    temporary
    ><v-list>
      <router-link to="/" class="nav-logo">
        <v-list-item prepend-avatar="/logo.svg" title="Cryptic"></v-list-item>
      </router-link>
    </v-list>

    <v-divider></v-divider>

    <v-list density="compact" nav>
      <v-list-item
        v-for="tab in tabs"
        :key="tab.key"
        :prepend-icon="tab.icon"
        :title="tab.title"
        :to="tab.to"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
export default {
  data: () => ({
    mobileDrawer: false,
    tabs: [
      {
        key: "notes",
        icon: "mdi-note-text-outline",
        title: "Notes",
        to: "/notes",
      },
    ],
  }),
};
</script>

<style scoped>
.nav-logo {
  text-decoration: none;
  color: inherit;
}
</style>
