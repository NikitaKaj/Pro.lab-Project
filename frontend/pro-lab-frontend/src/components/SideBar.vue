<template>
  <aside class="w-72 bg-[#0b2a5e] text-white flex flex-col p-6">
    <div class="flex items-center gap-3 mb-5">
      <div class="w-12 h-12 rounded-full bg-black/30 flex items-center justify-center text-xl">A</div>
      <div class="leading-tight ">
        <p class="font-semibold text-base">Aleksandrs Kunicins</p>
        <p class="text-xs text-white/70">Admin</p>
      </div>
    </div>
<!-- у меня тут не получается сделать отстпу mt-10 нужна помощь  -->
    <nav class="mt-10 space-y-2 text-[15px]">
      <button class="w-full flex items-center gap-4 px-5 py-3 rounded-md bg-[#1350b8]">
        <span class="text-xl">🏠</span><span class="font-semibold">Dashboard</span>
      </button>
      <button class="w-full flex items-center gap-4 px-5 py-3 rounded-md hover:bg-white/10">
        <span class="text-xl">🧾</span><span>Orders</span>
      </button>
      <button class="w-full flex items-center gap-4 px-5 py-3 rounded-md hover:bg-white/10">
        <span class="text-xl">🧭</span><span>Routes</span>
      </button>
      <button class="w-full flex items-center gap-4 px-5 py-3 rounded-md hover:bg-white/10">
        <span class="text-xl">👤</span><span>Couriers</span>
      </button>
      <button class="w-full flex items-center gap-4 px-5 py-3 rounded-md hover:bg-white/10">
        <span class="text-xl">👥</span><span>Clients</span>
      </button>
    </nav>
  </aside>
</template>

<script setup lang="ts">
</script>
