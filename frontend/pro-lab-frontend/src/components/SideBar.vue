<template>
  <aside class="w-72 bg-[#0b2a5e] text-white flex flex-col pl-6 pt-6 gap-10">
    <div class="flex items-center gap-3 mb-5">
      <div class="w-12 h-12 rounded-full bg-black/30 flex items-center justify-center text-xl">A</div>
      <div class="leading-tight ">
        <p class="font-semibold text-base">Developer</p>
        <p class="text-xs text-white/70">Admin</p>
      </div>
    </div>
    <nav class="space-y-2 text-[15px]">
      <RouterLink to="/" class="router-link">
        <span>🏠</span>
        <span>Dashboard</span>
      </RouterLink>

      <RouterLink to="/" class="router-link">
        <span>🧾</span>
        <span>Orders</span>
      </RouterLink>

      <RouterLink to="/routes" class="router-link">
        <span>🧭</span>
        <span>Routes</span>
      </RouterLink>

      <RouterLink to="/" class="router-link">
        <span>👤</span>
        <span>Couriers</span>
      </RouterLink>

      <RouterLink to="/" class="router-link">
        <span>👥</span>
        <span>Clients</span>
      </RouterLink>
    </nav>
  </aside>
</template>

<script setup lang="ts">
</script>
