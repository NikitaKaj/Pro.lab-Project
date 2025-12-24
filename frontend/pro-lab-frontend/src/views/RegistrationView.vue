<template>
  <div class="flex h-screen">
    <!-- Левая часть -->
    <div class="w-1/2 bg-gray-50 flex flex-col items-center justify-center">
      <div class="text-center">
        <img
          src="@/assets/images/RegistrationImage.jpg"
          alt="Delivery Illustration"
          class="w-full h-full"
        />
      </div>
    </div>

    <!-- Правая часть -->
    <div class="w-1/2 bg-blue-600 flex items-center justify-center">
      <div class="bg-white rounded-lg shadow-lg p-10 w-80">
        <h2 class="text-2xl font-bold text-center !mb-4 text-gray-800">
          Login
        </h2>
        <form @submit.prevent="handleLogin">
          <div class="!mb-4">
            <label class="block text-gray-600 !mb-2" for="username">
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

          <div class="!mb-2">
            <label class="block text-gray-600 !mb-2" for="password">
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
            class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition !mt-4"
            :disabled="loading"
          >
             {{ loading ? 'Processing...' : 'Login' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { AccountsClient } from '@/api-client/clients';
import { useRouter } from 'vue-router'

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

    console.log(res.accessToken)
    console.log(res.refreshToken)
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
