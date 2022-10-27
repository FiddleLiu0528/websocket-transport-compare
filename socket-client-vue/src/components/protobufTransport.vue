<script setup lang="ts">
import { onBeforeMount, onBeforeUnmount, ref } from "vue";
import protoRoot from "@/proto/proto";

const props = defineProps({
  rId: String,
});

let socket: WebSocket | null = null;

const requestActionList = ref<string[]>([]);
const receiveActionList = ref<string[]>([]);

const GenerateWebSocket = () => {
  console.log("GenerateWebSocket");

  const wsUrl = "ws://localhost:5000/ws-protobuf";

  socket = new WebSocket(wsUrl);

  socket.onopen = () => {
    console.log("open");
  };

  socket.onmessage = (msg) => {
    msg.data.arrayBuffer().then((buffer: ArrayBuffer) => {
      const base = protoRoot
        .lookupType("base.Base")
        .decode(new Uint8Array(buffer))
        .toJSON();

      if (base.action === "register") {
        const res = protoRoot
          .lookupType("register.Response")
          .decode(new Uint8Array(buffer))
          .toJSON();

        receiveActionList.value.push(JSON.stringify(res));
        return;
      } else if (base.action === "first") {
        const res = protoRoot
          .lookupType("first.Response")
          .decode(new Uint8Array(buffer))
          .toJSON();

        receiveActionList.value.push(JSON.stringify(res));
        return;
      }
    });
  };

  socket.onclose = () => {
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

  const registerRequestType = protoRoot.lookupType("register.Request");

  var buffer = registerRequestType
    .encode(registerRequestType.create(payload))
    .finish();

  socket?.send(buffer);
  requestActionList.value.push(JSON.stringify(payload));
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

  const firstRequestType = protoRoot.lookupType("first.Request");

  var buffer = firstRequestType
    .encode(firstRequestType.create(payload))
    .finish();

  socket?.send(buffer);
  requestActionList.value.push(JSON.stringify(payload));
};

onBeforeMount(() => {
  GenerateWebSocket();
});

onBeforeUnmount(() => {
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
