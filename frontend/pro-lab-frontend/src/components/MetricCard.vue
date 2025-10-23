<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    label: string
    value: string | number
    suffix?: string
    size?: 'sm' | 'lg'
  }>(),
  { size: 'lg' }
)

const wrapperClass = computed(() =>
  'border border-gray-200 rounded-lg shadow-sm ' + (props.size === 'sm' ? 'px-5 py-4' : 'px-6 py-4')
)
const labelClass  = computed(() => (props.size === 'sm' ? 'text-sm mb-1' : 'text-sm mb-1'))
const valueClass  = computed(() => (props.size === 'sm' ? 'text-3xl'     : 'text-3xl'))
const suffixClass = computed(() => (props.size === 'sm' ? 'text-base'    : 'text-base'))
</script>


<template>
  <div :class="wrapperClass">
    <div class="text-gray-600" :class="labelClass">{{ props.label }}</div>
    <div class="font-extrabold tracking-tight text-gray-900" :class="valueClass">
      {{ props.value }}
      <span v-if="props.suffix" class="font-semibold ml-1 text-gray-600" :class="suffixClass">
        {{ props.suffix }}
      </span>
    </div>
  </div>
</template>


