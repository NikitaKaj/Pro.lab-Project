<script setup lang="ts">
import { ref } from 'vue'
import Sidebar from '@/components/SideBar.vue'
import MapView from "@/components/MapView.vue";

// Пример данных, надо будет подключить API
const routes = ref([
  {
    address: 'Brivibas iela 1',
    city: 'Riga',
    time: '10:00 - 14:00',
    courier: 'Pierre Gasly',
    color: 'blue'
  },
  {
    address: 'Čaka iela 55',
    city: 'Riga',
    time: '11:30 - 12:00',
    courier: 'Fernando Alonso',
    color: 'orange'
  },
  {
    address: 'Valdemāra iela 1',
    city: 'Riga',
    time: '15:20 - 17:00',
    courier: 'Lando Norris',
    color: 'blue'
  },
  {
    address: 'Kuldīgas iela 4a',
    city: 'Riga',
    time: '8:00 - 11:00',
    courier: 'Max Verstappen',
    color: 'red'
  }
])
</script>

<template>
  <div class="flex min-h-screen bg-white text-gray-900">
    <Sidebar />

    <div class="flex-1 px-12 py-12">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-4xl font-extrabold">Routes</h1>
        <button class="bg-[#1673ea] hover:bg-[#105fc6] text-white font-semibold px-7 py-3.5 rounded-md shadow" @click="$emit('action')">
          Generate Optimal Route
        </button>
      </div>

      <div class="flex gap-6 pt-10">
        <!-- Карта -->
        <div class="w-2/3 bg-white rounded-lg border border-gray-200 shadow-sm h-[600px] flex items-center justify-center">
            <MapView />
        </div>

        <!-- Лист курьеров -->
        <div class="w-1/3 bg-white rounded-lg border border-gray-200 shadow-sm p-4">
          <h2 class="text-lg text-black font-semibold mb-4">Pienemšanas laiks</h2>
          <ul class="space-y-3">
            <li v-for="(r, i) in routes" :key="i" class="border-b pb-2">
              <div class="font-semibold text-black">{{ r.address }}</div>
              <div class="text-black text-sm">{{ r.city }}</div>
              <div class="flex items-center justify-between mt-1">
                <span class="text-sm px-2 py-1 rounded text-white" 
                      :class="{
                        'bg-blue-500': r.color === 'blue',
                        'bg-red-500': r.color === 'red',
                        'bg-yellow-500': r.color === 'orange'
                      }">
                  {{ r.time }}
                </span>
                <span class="text-sm text-gray-700">{{ r.courier }}</span>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>


