<script setup lang="ts">
withDefaults(defineProps<{
  courierName: string
  status: string
  avatar?: string
  summary: Array<{ icon: string; label: string; value: string | number }>
}>(), {
  avatar: 'https://i.pravatar.cc/100?img=15'
})
</script>

<template>
  <aside class="rounded-lg border border-gray-200 shadow-sm p-6">
    <h3 class="text-xl font-bold mb-5">Couriers</h3>
    <div class="flex items-center gap-4 mb-8">
      <img :src="avatar" class="w-12 h-12 rounded-full" alt="Courier" />
      <div>
        <p class="font-semibold leading-tight">{{ courierName }}</p>
        <p class="text-gray-600 text-sm leading-tight">{{ status }}</p>
      </div>
    </div>

    <h4 class="text-xl font-bold mb-4">Orders Summary</h4>
    <ul class="space-y-4 text-base">
      <li v-for="(row, i) in summary" :key="i" class="flex items-center justify-between">
        <span class="flex items-center gap-3">
          <span class="text-xl">{{ row.icon }}</span>
          <span class="font-semibold">{{ row.label }}</span>
        </span>
        <span class="font-extrabold text-xl">{{ row.value }}</span>
      </li>
    </ul>
  </aside>
</template>



