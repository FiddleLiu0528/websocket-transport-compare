<script setup lang="ts">
import pako from "pako";
import { onBeforeMount, onUnmounted, ref } from "vue";

const props = defineProps({
  rId: String,
});

let socket: WebSocket | null = null;

const requestActionList = ref<string[]>([]);
const receiveActionList = ref<string[]>([]);

const GenerateWebSocket = () => {
  console.log("GenerateWebSocket");

  const wsUrl = "ws://localhost:5000/ws-compress-json";

  socket = new WebSocket(wsUrl);

  socket.onopen = () => {
    console.log("open");
  };

  socket.onmessage = (msg) => {
    msg.data.arrayBuffer().then((text: any) => {
      const tempData = pako.inflateRaw(text, { to: "string" });
      const res = JSON.parse(tempData);

      if (res.action == "register") {
        receiveActionList.value.push(res);
        return;
      } else if (res.action == "first") {
        receiveActionList.value.push(res);
        return;
      }
    });
  };

  socket.onclose = function () {
    console.log("onclose");
  };
};

const UpdateUserId = () => {
  if (1 !== socket?.readyState) {
    console.log("socket state error");
    return;
  }

  const payload = {
    action: "register",
    id: props.rId?.toString(),
  };

  const requestAction = JSON.stringify(payload);
  const deflateRes = pako.deflateRaw(requestAction);

  socket.send(deflateRes);
  requestActionList.value.push(requestAction);
};

const FirstAction = () => {
  if (1 !== socket?.readyState) {
    console.log("socket state error");
    return;
  }

  const payload = {
    action: "first",
    mode: 1,
    pt: 1,
    sort: 1,
    keepL: 1,
    dc: 1,
  };

  const requestAction = JSON.stringify(payload);
  const deflateRes = pako.deflateRaw(requestAction);

  socket.send(deflateRes);
  requestActionList.value.push(requestAction);
};

onBeforeMount(() => {
  GenerateWebSocket();
});

onUnmounted(() => {
  console.log("onUnmounted");

  if (!socket) return;
  socket.close();
});
</script>

<template>
  <div><button @click="UpdateUserId">update user id</button>-{{ rId }}</div>

  <div>
    <h3>First Action</h3>
    <button @click="FirstAction">First Action</button>
  </div>

  <div>
    <div>
      <h3>request</h3>
      <div class="result">
        <div v-for="message in requestActionList" :key="message">
          {{ message }}
        </div>
      </div>
    </div>

    <div>
      <h3>response</h3>
      <div class="result">
        <div v-for="message in receiveActionList" :key="message">
          {{ message }}
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.result {
  border: red solid 1px;
  min-height: 20vh;
}
</style>
