<script setup lang="ts">
import { computed, onBeforeMount, defineAsyncComponent, ref } from "vue";

const targetComponent = ref("");

const rId = ref("");

const componentName = computed(() => {
  if (targetComponent.value == "JsonTransport") {
    return defineAsyncComponent(() => import("@/components/JsonTransport.vue"));
  } else if (targetComponent.value == "JsonCompressTransport") {
    return defineAsyncComponent(
      () => import("@/components/JsonCompressTransport.vue")
    );
  } else if (targetComponent.value == "protobufTransport") {
    return defineAsyncComponent(
      () => import("@/components/protobufTransport.vue")
    );
  }
  return null;
});

const SelectComponent = (name: string) => {
  if (targetComponent.value === name) {
    targetComponent.value = "";
    return;
  }
  targetComponent.value = name;
};

onBeforeMount(() => {
  rId.value = Math.random().toString().split(".")[1];
});
</script>

<template>
  <button @click="SelectComponent('JsonTransport')">Json Transport</button>
  <button @click="SelectComponent('JsonCompressTransport')">
    Json Compress Transport
  </button>
  <button @click="SelectComponent('protobufTransport')">
    protobuf Transport
  </button>
  <div>
    modify id :
    <input type="text" v-model="rId" />
  </div>

  <hr />

  <component :is="componentName" :rId="rId" />
</template>

<style scoped></style>
