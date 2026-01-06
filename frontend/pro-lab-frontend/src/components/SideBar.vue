<template>
  <!-- Desktop sidebar -->
  <aside class="hidden md:flex w-72 bg-[#0b2a5e] text-white flex-col pl-6 pt-6 gap-10">
    <div class="flex items-center gap-3 mb-5">
      <div class="w-12 h-12 rounded-full bg-black/30 flex items-center justify-center text-xl">A</div>
      <div class="leading-tight">
        <p class="font-semibold text-base">Developer</p>
        <p class="text-xs text-white/70">Admin</p>
      </div>
    </div>

    <nav class="space-y-2 text-[15px] pr-4">
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

  <div class="md:hidden">
    <div
      v-if="open"
      class="fixed inset-0 bg-black/40 z-40"
      @click="close"
    />


    <transition name="slide">
      <aside
        v-if="open"
        class="fixed z-50 inset-y-0 left-0 w-[82vw] max-w-[320px] bg-[#0b2a5e] text-white flex flex-col pl-6 pt-6 gap-8"
      >
        <div class="flex items-center justify-between pr-4">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 rounded-full bg-black/30 flex items-center justify-center text-xl">A</div>
            <div class="leading-tight">
              <p class="font-semibold text-base">Developer</p>
              <p class="text-xs text-white/70">Admin</p>
            </div>
          </div>

          <button
            type="button"
            class="p-2 rounded-md hover:bg-white/10"
            @click="close"
            aria-label="Close menu"
          >
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>

        <nav class="space-y-2 text-[15px] pr-4">
          <RouterLink to="/dashboard" class="router-link" @click="close">
            <HomeIcon class="w-5 h-5" />
            <span>Dashboard</span>
          </RouterLink>

          <RouterLink to="/orders" class="router-link" @click="close">
            <ShoppingCartIcon class="w-5 h-5" />
            <span>Orders</span>
          </RouterLink>

          <RouterLink to="/routes" class="router-link" @click="close">
            <MapIcon class="w-5 h-5" />
            <span>Routes</span>
          </RouterLink>

          <RouterLink to="/couriers" class="router-link" @click="close">
            <TruckIcon class="w-5 h-5" />
            <span>Couriers</span>
          </RouterLink>

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
    </transition>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import {
  HomeIcon,
  ArrowRightOnRectangleIcon,
  ShoppingCartIcon,
  TruckIcon,
  MapIcon,
  XMarkIcon,
} from '@heroicons/vue/24/outline'
import { AccountsClient } from '@/api-client/clients'
import { useRouter } from 'vue-router'

const open = defineModel<boolean>('open', { default: false })

const accountsClient = new AccountsClient()
const router = useRouter()

function close() {
  open.value = false
}

const handleLogout = async () => {
  try {
    await accountsClient.logout()

    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    close()
    router.push('/')
  } catch (e) {
    alert("You're not authorized")
    console.error(e)
  }
}
</script>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.2s ease;
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(-100%);
}
</style>
