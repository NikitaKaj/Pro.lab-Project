<template>
  <div class="min-h-screen flex flex-col md:flex-row">
    <div class="w-full md:w-1/2 bg-gray-50 flex items-center justify-center overflow-hidden h-[260px] md:h-auto">
      <div class="w-full h-full">
        <img
          src="@/assets/images/RegistrationImage.jpg"
          alt="Delivery Illustration"
          class="w-full h-full object-cover object-center"
        />
      </div>
    </div>

    <div class="w-full md:w-1/2 bg-blue-600 flex items-center justify-center px-4 py-10 md:p-0">
      <div class="bg-white rounded-lg shadow-lg p-8 sm:p-10 w-full max-w-[360px]">
        <h2 class="text-2xl font-bold text-center mb-4 text-gray-800">
          Login
        </h2>

        <form @submit.prevent="handleLogin">
          <div class="mb-4">
            <label class="block text-gray-600 mb-2" for="username">
              User Name
            </label>
            <input
              id="username"
              v-model="username"
              type="text"
              placeholder="Enter your username"
              class="w-full border border-gray-400 rounded px-3 py-2 placeholder-gray-400 text-black"
            />
          </div>

          <div class="mb-2">
            <label class="block text-gray-600 mb-2" for="password">
              Password
            </label>
            <input
              id="password"
              v-model="password"
              type="password"
              placeholder="Enter your password"
              class="w-full border border-gray-400 rounded px-3 py-2 placeholder-gray-400 text-black"
            />
          </div>

          <button
            type="submit"
            class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition mt-4 disabled:opacity-70"
            :disabled="loading"
          >
            {{ loading ? 'Processing...' : 'Login' }}
          </button>
        </form>

        <div class="text-center mt-4">
          <text class="text-gray-500 text-sm">Login: admin<br />Password: admin</text>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>

import { ref } from "vue";
import { AccountsClient } from '@/api-client/clients';
import { useRouter } from 'vue-router'

const sidebarOpen = ref(false)

const accountsClient = new AccountsClient();
const router = useRouter()

const username = ref("");
const password = ref("");
const loading = ref(false)
const error = ref("")

const handleLogin = async () => {
  error.value = ''
  loading.value = true

  try {
    const res = await accountsClient.login({
      email: username.value,
      password: password.value,
    })

    localStorage.setItem('accessToken', res.accessToken ?? '')
    localStorage.setItem('refreshToken', res.refreshToken ?? '')
    router.push('/dashboard')

  } catch (e) {
    console.error(e)
    error.value = 'Incorrect login or password'
    alert(error.value);
  } finally {
    loading.value = false
  }
}
</script>
