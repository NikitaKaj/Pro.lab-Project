<script setup lang="ts">
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import TopMetricsRow from '@/components/TopMetricsRow.vue'
import SmallMetricsColumn from '@/components/SmallMetricsColumn.vue'
import OrdersChart from '@/components/OrderCharts.vue'
import CouriersPanel from "@/components/CouriersPanel.vue";

function onGenerateRoute() {
  alert('Generating optimal route…')
}
</script>

<template>
  <div class="flex min-h-screen bg-white text-gray-900">
    <Sidebar />
    
    <main class="flex-1 overflow-auto">
      <div class="w-full px-12 py-12">
        <PageHeader title="Dashboard" @action="onGenerateRoute" />
        <TopMetricsRow
          :items="[
            { label: 'Orders Today', value: 128 },
            { label: 'Completed', value: 85 },
            { label: 'In Progress', value: 36 },
            { label: 'Canceled', value: 7 },
          ]"
        />

        <div class="mt-8 grid gap-6 xl:grid-cols-[300px,1fr,360px] 2xl:grid-cols-[320px,1fr,380px]">
          <SmallMetricsColumn
            :cards="[
              { label: 'Avg. Delivery Time', value: '15,2', suffix: 'min' },
              { label: 'New Clients Today', value: '15' },
              { label: 'Additional', value: '15' },
            ]"
          />

          <OrdersChart class="rounded-lg border border-gray-200 shadow-sm"
                       :labels="['Mon','Tue','Wed','Thu','Fri','Sat','Sun']"
                       :series="[50,75,60,90,100,80,120]" />

          <CouriersPanel
            courier-name="Churka"
            status="In Transit"
            :summary="[
              { icon: '📄', label: 'Orders', value: 350 },
              { icon: '🚚', label: 'Shipped', value: 240 },
              { icon: '🌀', label: 'Pending', value: 240 },
              { icon: '🚫', label: 'Canceled', value: 240 },
            ]"
          />
        </div>
      </div>
    </main>
  </div>
</template>


