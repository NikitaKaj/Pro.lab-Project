<template>
  <div class="flex h-screen">
    <!-- Левая часть -->
    <div class="w-1/2 bg-gray-50 flex flex-col items-center justify-center">
      <div class="max-w-sm text-center">
        <!-- <img
          src="/delivery-illustration.png"
          alt="Delivery Illustration"
          class="w-full"
        /> -->
      </div>
    </div>

    <!-- Правая часть -->
    <div class="w-1/2 bg-blue-600 flex items-center justify-center">
      <div class="bg-white rounded-lg shadow-lg p-10 w-80">
        <h2 class="text-2xl font-bold text-center !mb-4 text-gray-800">
          Login
        </h2>
        <form @submit.prevent="login">
          <div class="!mb-4">
            <label class="block text-gray-600 !mb-2" for="username">
              User Name
            </label>
            <input
              id="username"
              v-model="username"
              type="text"
              placeholder="Enter your username"
              class="w-full border border-gray-400 rounded px-3 py-2 placeholder-gray-400"
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
              class="w-full border border-gray-400 rounded px-3 py-2 placeholder-gray-400"
            />
          </div>

          <button
            type="submit"
            class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition !mt-4"
          >
            Login
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";

const username = ref("");
const password = ref("");

function login() {
  alert(`Username: ${username.value}\nPassword: ${password.value}`);
}
</script>
