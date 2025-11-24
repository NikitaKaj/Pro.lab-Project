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
      <RouterLink to="/dashboard" class="router-link">
        <HomeIcon class="w-5 h-5" />
        <span>Dashboard</span>
      </RouterLink>

      <RouterLink to="/orders" class="router-link">
        <ShoppingCartIcon class="w-5 h-5" />
        <span>Orders</span>
      </RouterLink>

      <RouterLink to="/routes" class="router-link">
        <MapIcon class="w-5 h-5" />
        <span>Routes</span>
      </RouterLink>

      <RouterLink to="/couriers" class="router-link">
        <TruckIcon class="w-5 h-5" />
        <span>Couriers</span>
      </RouterLink>

      <!-- <RouterLink to="/dashboard" class="router-link">
        <UserIcon class="w-5 h-5" />
        <span>Clients</span>
      </RouterLink> -->

      <button
        type="button"
        @click="handleLogout"
        class="flex items-center gap-3 px-4 py-2 rounded-lg w-full hover:bg-white/10 transition-colors cursor-pointer"
      >
        <ArrowRightOnRectangleIcon class="w-5 h-5" />
        <span>Logout</span>
      </button>
    </nav>
  </aside>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { HomeIcon, UserIcon, ArrowRightOnRectangleIcon, ShoppingCartIcon, TruckIcon, MapIcon } from '@heroicons/vue/24/outline';
import { AccountsClient } from '@/api-client/clients';
import { useRouter } from 'vue-router'

const accountsClient = new AccountsClient();
const router = useRouter();

const handleLogout = async () => {
  try {
    await accountsClient.logout();

    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    router.push('/')

  } catch (e) {
    alert("You're not authorized");
    console.error(e)
  }
}
</script>
