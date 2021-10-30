(()=>{var e={826:()=>{(()=>{"use strict";var e={92:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},691:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},696:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},706:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},939:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},73:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},164:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},920:function(e,t,n){var o=this&&this.__createBinding||(Object.create?function(e,t,n,o){void 0===o&&(o=n),Object.defineProperty(e,o,{enumerable:!0,get:function(){return t[n]}})}:function(e,t,n,o){void 0===o&&(o=n),e[o]=t[n]}),s=this&&this.__exportStar||function(e,t){for(var n in e)"default"===n||Object.prototype.hasOwnProperty.call(t,n)||o(t,e,n)};Object.defineProperty(t,"__esModule",{value:!0}),s(n(92),t),s(n(691),t),s(n(696),t),s(n(706),t),s(n(939),t),s(n(73),t),s(n(164),t),s(n(609),t),s(n(999),t),s(n(24),t),s(n(315),t),s(n(296),t),s(n(933),t),s(n(196),t),s(n(464),t),s(n(166),t),s(n(163),t),s(n(789),t),s(n(817),t),s(n(379),t),s(n(323),t),s(n(679),t)},609:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},999:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},24:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},315:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},296:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},933:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},196:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},464:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},166:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.InputDeviceDetailsRequestType=void 0,t.InputDeviceDetailsRequestType="InputDeviceDetails"},163:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.InputDeviceFeedbackResponseType=void 0,t.InputDeviceFeedbackResponseType="InputDeviceFeedback"},789:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.InputDeviceInputRequestType=void 0,t.InputDeviceInputRequestType="InputDeviceInput"},817:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.InputDeviceInputResponseType=void 0,t.InputDeviceInputResponseType="InputDeviceInputFeedback"},379:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},323:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})},679:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0})}},t={};!function n(o){var s=t[o];if(void 0!==s)return s.exports;var i=t[o]={exports:{}};return e[o].call(i.exports,i,i.exports,n),i.exports}(920)})()},920:function(e,t,n){"use strict";var o=this&&this.__createBinding||(Object.create?function(e,t,n,o){void 0===o&&(o=n),Object.defineProperty(e,o,{enumerable:!0,get:function(){return t[n]}})}:function(e,t,n,o){void 0===o&&(o=n),e[o]=t[n]}),s=this&&this.__exportStar||function(e,t){for(var n in e)"default"===n||Object.prototype.hasOwnProperty.call(t,n)||o(t,e,n)};Object.defineProperty(t,"__esModule",{value:!0}),s(n(928),t),s(n(61),t),s(n(320),t),s(n(706),t),s(n(305),t),s(n(986),t),s(n(392),t)},61:(e,t,n)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.infoClient=void 0;const o=n(928);t.infoClient={getInfo:()=>o.http.get("/info")}},928:(e,t)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.http=t.HttpService=void 0;class n{initialize(e,t){this.host=e,this.port=t}get(e){return fetch(`http://${this.host}:${this.port}/api${e}`,{method:"GET",headers:{"Content-Type":"application/json"}}).then((t=>this.readBody(t,`/api${e}`)))}post(e,t){return fetch(`http://${this.host}:${this.port}/api${e}`,{method:"POST",body:null!=t?JSON.stringify(t):null,headers:{"Content-Type":"application/json"}}).then((t=>this.readBody(t,`/api${e}`)))}put(e,t){return fetch(`http://${this.host}:${this.port}/api${e}`,{method:"PUT",body:null!=t?JSON.stringify(t):null,headers:{"Content-Type":"application/json"}}).then((t=>this.readBody(t,`/api${e}`)))}delete(e,t){return fetch(`http://${this.host}:${this.port}/api${e}`,{method:"DELETE",body:null!=t?JSON.stringify(t):null,headers:{"Content-Type":"application/json"}}).then((t=>this.readBody(t,`/api${e}`)))}readBody(e,t){return e.text().then((e=>{if(e)try{return JSON.parse(e)}catch(n){throw new Error(`Failed to parse JSON from response body from ${t}. Response body: `+(e.length>1e3?e.substring(0,1e3)+"...":e))}return null}))}}t.HttpService=n,t.http=new n},320:(e,t,n)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.inputsClient=void 0;const o=n(928);t.inputsClient={getInputs:()=>o.http.get("/inputs"),getInput:e=>o.http.get(`/inputs/${e}`)}},706:(e,t,n)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.notificationClient=void 0;const o=n(928);t.notificationClient={getNotifications:()=>o.http.get("/notifications"),acknowledge:e=>o.http.put(`/notifications/${e}/acknowledge`)}},986:(e,t,n)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.inputDeviceClient=void 0;const o=n(826),s=n(305);t.inputDeviceClient={connect:(e,t)=>s.websocket.connect("InputDevice",(e=>{e.type===o.InputDeviceFeedbackResponseType&&t(e)})).then((t=>(t.sendMessage(e),t))).then((e=>({sendInput:t=>e.sendMessage(t)})))}},392:(e,t,n)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.inputDeviceFeedbackClient=void 0;const o=n(826),s=n(305);t.inputDeviceFeedbackClient={connect:(e,t)=>s.websocket.connect(`InputDevice/${e}`,(e=>{e.type===o.InputDeviceInputResponseType&&t(e)})).then((()=>null))}},305:(e,t)=>{"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.websocket=t.WebSocketSession=t.WebSocketService=void 0;class n{initialize(e,t){this.host=e,this.port=t}connect(e,t){return new Promise(((n,s)=>{const i=`ws://${this.host}:${this.port}/websocket/${e}`,r=new WebSocket(i);let c,p;r.onopen=e=>{c=new o(r,i),this.onOpen(e),p=setInterval((()=>{c.sendMessage({type:"Ping",timestamp:(new Date).getTime()})}),5e3),n(c)},r.onerror=e=>{this.onError(e),c||s(e)},r.onclose=e=>this.onClose(p,e),r.onmessage=e=>{const n=JSON.parse(e.data);this.onMessage(c,n)||t(n)}}))}onOpen(e){console.info("Connected to "+this.host+":"+this.port)}onError(e){const t=e.message;console.error(t)}onClose(e,t){console.info("Disconnected from "+this.host+":"+this.port),e&&clearInterval(e)}onMessage(e,t){return"Debug"===t.type?(console.debug(t.data),!0):"Ping"===t.type?(e.sendMessage({type:"Pong",timestamp:(new Date).getTime()}),!0):"Pong"===t.type&&(console.debug(`Delay is ${(new Date).getTime()-t.timestamp} ms`),!0)}}t.WebSocketService=n;class o{constructor(e,t){this.websocket=e,this.url=t}close(){this.websocket.close()}isReady(){return this.websocket&&this.websocket.readyState===WebSocket.OPEN}sendMessage(e){this.websocket.send(JSON.stringify(e))}sendDebug(e){this.sendMessage({type:"Debug",data:e})}}t.WebSocketSession=o,t.websocket=new n}},t={};!function n(o){var s=t[o];if(void 0!==s)return s.exports;var i=t[o]={exports:{}};return e[o].call(i.exports,i,i.exports,n),i.exports}(920)})();