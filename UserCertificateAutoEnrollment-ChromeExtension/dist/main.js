(()=>{var e={669:(e,t,n)=>{n(609)},448:(e,t,n)=>{"use strict";var r=n(867),o=n(26),s=n(372),i=n(327),a=n(97),c=n(109),u=n(985),l=n(874),f=n(648),d=n(644),p=n(205);e.exports=function(e){return new Promise((function(t,n){var h,m=e.data,g=e.headers,v=e.responseType;function y(){e.cancelToken&&e.cancelToken.unsubscribe(h),e.signal&&e.signal.removeEventListener("abort",h)}r.isFormData(m)&&r.isStandardBrowserEnv()&&delete g["Content-Type"];var E=new XMLHttpRequest;if(e.auth){var w=e.auth.username||"",b=e.auth.password?unescape(encodeURIComponent(e.auth.password)):"";g.Authorization="Basic "+btoa(w+":"+b)}var O=a(e.baseURL,e.url);function R(){if(E){var r="getAllResponseHeaders"in E?c(E.getAllResponseHeaders()):null,s={data:v&&"text"!==v&&"json"!==v?E.response:E.responseText,status:E.status,statusText:E.statusText,headers:r,config:e,request:E};o((function(e){t(e),y()}),(function(e){n(e),y()}),s),E=null}}if(E.open(e.method.toUpperCase(),i(O,e.params,e.paramsSerializer),!0),E.timeout=e.timeout,"onloadend"in E?E.onloadend=R:E.onreadystatechange=function(){E&&4===E.readyState&&(0!==E.status||E.responseURL&&0===E.responseURL.indexOf("file:"))&&setTimeout(R)},E.onabort=function(){E&&(n(new f("Request aborted",f.ECONNABORTED,e,E)),E=null)},E.onerror=function(){n(new f("Network Error",f.ERR_NETWORK,e,E,E)),E=null},E.ontimeout=function(){var t=e.timeout?"timeout of "+e.timeout+"ms exceeded":"timeout exceeded",r=e.transitional||l;e.timeoutErrorMessage&&(t=e.timeoutErrorMessage),n(new f(t,r.clarifyTimeoutError?f.ETIMEDOUT:f.ECONNABORTED,e,E)),E=null},r.isStandardBrowserEnv()){var S=(e.withCredentials||u(O))&&e.xsrfCookieName?s.read(e.xsrfCookieName):void 0;S&&(g[e.xsrfHeaderName]=S)}"setRequestHeader"in E&&r.forEach(g,(function(e,t){void 0===m&&"content-type"===t.toLowerCase()?delete g[t]:E.setRequestHeader(t,e)})),r.isUndefined(e.withCredentials)||(E.withCredentials=!!e.withCredentials),v&&"json"!==v&&(E.responseType=e.responseType),"function"==typeof e.onDownloadProgress&&E.addEventListener("progress",e.onDownloadProgress),"function"==typeof e.onUploadProgress&&E.upload&&E.upload.addEventListener("progress",e.onUploadProgress),(e.cancelToken||e.signal)&&(h=function(e){E&&(n(!e||e&&e.type?new d:e),E.abort(),E=null)},e.cancelToken&&e.cancelToken.subscribe(h),e.signal&&(e.signal.aborted?h():e.signal.addEventListener("abort",h))),m||(m=null);var C=p(O);C&&-1===["http","https","file"].indexOf(C)?n(new f("Unsupported protocol "+C+":",f.ERR_BAD_REQUEST,e)):E.send(m)}))}},609:(e,t,n)=>{"use strict";var r=n(867),o=n(849),s=n(321),i=n(185),a=function e(t){var n=new s(t),a=o(s.prototype.request,n);return r.extend(a,s.prototype,n),r.extend(a,n),a.create=function(n){return e(i(t,n))},a}(n(546));a.Axios=s,a.CanceledError=n(644),a.CancelToken=n(972),a.isCancel=n(502),a.VERSION=n(288).version,a.toFormData=n(675),a.AxiosError=n(648),a.Cancel=a.CanceledError,a.all=function(e){return Promise.all(e)},a.spread=n(713),a.isAxiosError=n(268),e.exports=a,e.exports.default=a},972:(e,t,n)=>{"use strict";var r=n(644);function o(e){if("function"!=typeof e)throw new TypeError("executor must be a function.");var t;this.promise=new Promise((function(e){t=e}));var n=this;this.promise.then((function(e){if(n._listeners){var t,r=n._listeners.length;for(t=0;t<r;t++)n._listeners[t](e);n._listeners=null}})),this.promise.then=function(e){var t,r=new Promise((function(e){n.subscribe(e),t=e})).then(e);return r.cancel=function(){n.unsubscribe(t)},r},e((function(e){n.reason||(n.reason=new r(e),t(n.reason))}))}o.prototype.throwIfRequested=function(){if(this.reason)throw this.reason},o.prototype.subscribe=function(e){this.reason?e(this.reason):this._listeners?this._listeners.push(e):this._listeners=[e]},o.prototype.unsubscribe=function(e){if(this._listeners){var t=this._listeners.indexOf(e);-1!==t&&this._listeners.splice(t,1)}},o.source=function(){var e;return{token:new o((function(t){e=t})),cancel:e}},e.exports=o},644:(e,t,n)=>{"use strict";var r=n(648);function o(e){r.call(this,null==e?"canceled":e,r.ERR_CANCELED),this.name="CanceledError"}n(867).inherits(o,r,{__CANCEL__:!0}),e.exports=o},502:e=>{"use strict";e.exports=function(e){return!(!e||!e.__CANCEL__)}},321:(e,t,n)=>{"use strict";var r=n(867),o=n(327),s=n(782),i=n(572),a=n(185),c=n(97),u=n(875),l=u.validators;function f(e){this.defaults=e,this.interceptors={request:new s,response:new s}}f.prototype.request=function(e,t){"string"==typeof e?(t=t||{}).url=e:t=e||{},(t=a(this.defaults,t)).method?t.method=t.method.toLowerCase():this.defaults.method?t.method=this.defaults.method.toLowerCase():t.method="get";var n=t.transitional;void 0!==n&&u.assertOptions(n,{silentJSONParsing:l.transitional(l.boolean),forcedJSONParsing:l.transitional(l.boolean),clarifyTimeoutError:l.transitional(l.boolean)},!1);var r=[],o=!0;this.interceptors.request.forEach((function(e){"function"==typeof e.runWhen&&!1===e.runWhen(t)||(o=o&&e.synchronous,r.unshift(e.fulfilled,e.rejected))}));var s,c=[];if(this.interceptors.response.forEach((function(e){c.push(e.fulfilled,e.rejected)})),!o){var f=[i,void 0];for(Array.prototype.unshift.apply(f,r),f=f.concat(c),s=Promise.resolve(t);f.length;)s=s.then(f.shift(),f.shift());return s}for(var d=t;r.length;){var p=r.shift(),h=r.shift();try{d=p(d)}catch(e){h(e);break}}try{s=i(d)}catch(e){return Promise.reject(e)}for(;c.length;)s=s.then(c.shift(),c.shift());return s},f.prototype.getUri=function(e){e=a(this.defaults,e);var t=c(e.baseURL,e.url);return o(t,e.params,e.paramsSerializer)},r.forEach(["delete","get","head","options"],(function(e){f.prototype[e]=function(t,n){return this.request(a(n||{},{method:e,url:t,data:(n||{}).data}))}})),r.forEach(["post","put","patch"],(function(e){function t(t){return function(n,r,o){return this.request(a(o||{},{method:e,headers:t?{"Content-Type":"multipart/form-data"}:{},url:n,data:r}))}}f.prototype[e]=t(),f.prototype[e+"Form"]=t(!0)})),e.exports=f},648:(e,t,n)=>{"use strict";var r=n(867);function o(e,t,n,r,o){Error.call(this),this.message=e,this.name="AxiosError",t&&(this.code=t),n&&(this.config=n),r&&(this.request=r),o&&(this.response=o)}r.inherits(o,Error,{toJSON:function(){return{message:this.message,name:this.name,description:this.description,number:this.number,fileName:this.fileName,lineNumber:this.lineNumber,columnNumber:this.columnNumber,stack:this.stack,config:this.config,code:this.code,status:this.response&&this.response.status?this.response.status:null}}});var s=o.prototype,i={};["ERR_BAD_OPTION_VALUE","ERR_BAD_OPTION","ECONNABORTED","ETIMEDOUT","ERR_NETWORK","ERR_FR_TOO_MANY_REDIRECTS","ERR_DEPRECATED","ERR_BAD_RESPONSE","ERR_BAD_REQUEST","ERR_CANCELED"].forEach((function(e){i[e]={value:e}})),Object.defineProperties(o,i),Object.defineProperty(s,"isAxiosError",{value:!0}),o.from=function(e,t,n,i,a,c){var u=Object.create(s);return r.toFlatObject(e,u,(function(e){return e!==Error.prototype})),o.call(u,e.message,t,n,i,a),u.name=e.name,c&&Object.assign(u,c),u},e.exports=o},782:(e,t,n)=>{"use strict";var r=n(867);function o(){this.handlers=[]}o.prototype.use=function(e,t,n){return this.handlers.push({fulfilled:e,rejected:t,synchronous:!!n&&n.synchronous,runWhen:n?n.runWhen:null}),this.handlers.length-1},o.prototype.eject=function(e){this.handlers[e]&&(this.handlers[e]=null)},o.prototype.forEach=function(e){r.forEach(this.handlers,(function(t){null!==t&&e(t)}))},e.exports=o},97:(e,t,n)=>{"use strict";var r=n(793),o=n(303);e.exports=function(e,t){return e&&!r(t)?o(e,t):t}},572:(e,t,n)=>{"use strict";var r=n(867),o=n(527),s=n(502),i=n(546),a=n(644);function c(e){if(e.cancelToken&&e.cancelToken.throwIfRequested(),e.signal&&e.signal.aborted)throw new a}e.exports=function(e){return c(e),e.headers=e.headers||{},e.data=o.call(e,e.data,e.headers,e.transformRequest),e.headers=r.merge(e.headers.common||{},e.headers[e.method]||{},e.headers),r.forEach(["delete","get","head","post","put","patch","common"],(function(t){delete e.headers[t]})),(e.adapter||i.adapter)(e).then((function(t){return c(e),t.data=o.call(e,t.data,t.headers,e.transformResponse),t}),(function(t){return s(t)||(c(e),t&&t.response&&(t.response.data=o.call(e,t.response.data,t.response.headers,e.transformResponse))),Promise.reject(t)}))}},185:(e,t,n)=>{"use strict";var r=n(867);e.exports=function(e,t){t=t||{};var n={};function o(e,t){return r.isPlainObject(e)&&r.isPlainObject(t)?r.merge(e,t):r.isPlainObject(t)?r.merge({},t):r.isArray(t)?t.slice():t}function s(n){return r.isUndefined(t[n])?r.isUndefined(e[n])?void 0:o(void 0,e[n]):o(e[n],t[n])}function i(e){if(!r.isUndefined(t[e]))return o(void 0,t[e])}function a(n){return r.isUndefined(t[n])?r.isUndefined(e[n])?void 0:o(void 0,e[n]):o(void 0,t[n])}function c(n){return n in t?o(e[n],t[n]):n in e?o(void 0,e[n]):void 0}var u={url:i,method:i,data:i,baseURL:a,transformRequest:a,transformResponse:a,paramsSerializer:a,timeout:a,timeoutMessage:a,withCredentials:a,adapter:a,responseType:a,xsrfCookieName:a,xsrfHeaderName:a,onUploadProgress:a,onDownloadProgress:a,decompress:a,maxContentLength:a,maxBodyLength:a,beforeRedirect:a,transport:a,httpAgent:a,httpsAgent:a,cancelToken:a,socketPath:a,responseEncoding:a,validateStatus:c};return r.forEach(Object.keys(e).concat(Object.keys(t)),(function(e){var t=u[e]||s,o=t(e);r.isUndefined(o)&&t!==c||(n[e]=o)})),n}},26:(e,t,n)=>{"use strict";var r=n(648);e.exports=function(e,t,n){var o=n.config.validateStatus;n.status&&o&&!o(n.status)?t(new r("Request failed with status code "+n.status,[r.ERR_BAD_REQUEST,r.ERR_BAD_RESPONSE][Math.floor(n.status/100)-4],n.config,n.request,n)):e(n)}},527:(e,t,n)=>{"use strict";var r=n(867),o=n(546);e.exports=function(e,t,n){var s=this||o;return r.forEach(n,(function(n){e=n.call(s,e,t)})),e}},546:(e,t,n)=>{"use strict";var r=n(867),o=n(16),s=n(648),i=n(874),a=n(675),c={"Content-Type":"application/x-www-form-urlencoded"};function u(e,t){!r.isUndefined(e)&&r.isUndefined(e["Content-Type"])&&(e["Content-Type"]=t)}var l,f={transitional:i,adapter:(("undefined"!=typeof XMLHttpRequest||"undefined"!=typeof process&&"[object process]"===Object.prototype.toString.call(process))&&(l=n(448)),l),transformRequest:[function(e,t){if(o(t,"Accept"),o(t,"Content-Type"),r.isFormData(e)||r.isArrayBuffer(e)||r.isBuffer(e)||r.isStream(e)||r.isFile(e)||r.isBlob(e))return e;if(r.isArrayBufferView(e))return e.buffer;if(r.isURLSearchParams(e))return u(t,"application/x-www-form-urlencoded;charset=utf-8"),e.toString();var n,s=r.isObject(e),i=t&&t["Content-Type"];if((n=r.isFileList(e))||s&&"multipart/form-data"===i){var c=this.env&&this.env.FormData;return a(n?{"files[]":e}:e,c&&new c)}return s||"application/json"===i?(u(t,"application/json"),function(e,t,n){if(r.isString(e))try{return(0,JSON.parse)(e),r.trim(e)}catch(e){if("SyntaxError"!==e.name)throw e}return(0,JSON.stringify)(e)}(e)):e}],transformResponse:[function(e){var t=this.transitional||f.transitional,n=t&&t.silentJSONParsing,o=t&&t.forcedJSONParsing,i=!n&&"json"===this.responseType;if(i||o&&r.isString(e)&&e.length)try{return JSON.parse(e)}catch(e){if(i){if("SyntaxError"===e.name)throw s.from(e,s.ERR_BAD_RESPONSE,this,null,this.response);throw e}}return e}],timeout:0,xsrfCookieName:"XSRF-TOKEN",xsrfHeaderName:"X-XSRF-TOKEN",maxContentLength:-1,maxBodyLength:-1,env:{FormData:n(623)},validateStatus:function(e){return e>=200&&e<300},headers:{common:{Accept:"application/json, text/plain, */*"}}};r.forEach(["delete","get","head"],(function(e){f.headers[e]={}})),r.forEach(["post","put","patch"],(function(e){f.headers[e]=r.merge(c)})),e.exports=f},874:e=>{"use strict";e.exports={silentJSONParsing:!0,forcedJSONParsing:!0,clarifyTimeoutError:!1}},288:e=>{e.exports={version:"0.27.2"}},849:e=>{"use strict";e.exports=function(e,t){return function(){for(var n=new Array(arguments.length),r=0;r<n.length;r++)n[r]=arguments[r];return e.apply(t,n)}}},327:(e,t,n)=>{"use strict";var r=n(867);function o(e){return encodeURIComponent(e).replace(/%3A/gi,":").replace(/%24/g,"$").replace(/%2C/gi,",").replace(/%20/g,"+").replace(/%5B/gi,"[").replace(/%5D/gi,"]")}e.exports=function(e,t,n){if(!t)return e;var s;if(n)s=n(t);else if(r.isURLSearchParams(t))s=t.toString();else{var i=[];r.forEach(t,(function(e,t){null!=e&&(r.isArray(e)?t+="[]":e=[e],r.forEach(e,(function(e){r.isDate(e)?e=e.toISOString():r.isObject(e)&&(e=JSON.stringify(e)),i.push(o(t)+"="+o(e))})))})),s=i.join("&")}if(s){var a=e.indexOf("#");-1!==a&&(e=e.slice(0,a)),e+=(-1===e.indexOf("?")?"?":"&")+s}return e}},303:e=>{"use strict";e.exports=function(e,t){return t?e.replace(/\/+$/,"")+"/"+t.replace(/^\/+/,""):e}},372:(e,t,n)=>{"use strict";var r=n(867);e.exports=r.isStandardBrowserEnv()?{write:function(e,t,n,o,s,i){var a=[];a.push(e+"="+encodeURIComponent(t)),r.isNumber(n)&&a.push("expires="+new Date(n).toGMTString()),r.isString(o)&&a.push("path="+o),r.isString(s)&&a.push("domain="+s),!0===i&&a.push("secure"),document.cookie=a.join("; ")},read:function(e){var t=document.cookie.match(new RegExp("(^|;\\s*)("+e+")=([^;]*)"));return t?decodeURIComponent(t[3]):null},remove:function(e){this.write(e,"",Date.now()-864e5)}}:{write:function(){},read:function(){return null},remove:function(){}}},793:e=>{"use strict";e.exports=function(e){return/^([a-z][a-z\d+\-.]*:)?\/\//i.test(e)}},268:(e,t,n)=>{"use strict";var r=n(867);e.exports=function(e){return r.isObject(e)&&!0===e.isAxiosError}},985:(e,t,n)=>{"use strict";var r=n(867);e.exports=r.isStandardBrowserEnv()?function(){var e,t=/(msie|trident)/i.test(navigator.userAgent),n=document.createElement("a");function o(e){var r=e;return t&&(n.setAttribute("href",r),r=n.href),n.setAttribute("href",r),{href:n.href,protocol:n.protocol?n.protocol.replace(/:$/,""):"",host:n.host,search:n.search?n.search.replace(/^\?/,""):"",hash:n.hash?n.hash.replace(/^#/,""):"",hostname:n.hostname,port:n.port,pathname:"/"===n.pathname.charAt(0)?n.pathname:"/"+n.pathname}}return e=o(window.location.href),function(t){var n=r.isString(t)?o(t):t;return n.protocol===e.protocol&&n.host===e.host}}():function(){return!0}},16:(e,t,n)=>{"use strict";var r=n(867);e.exports=function(e,t){r.forEach(e,(function(n,r){r!==t&&r.toUpperCase()===t.toUpperCase()&&(e[t]=n,delete e[r])}))}},623:e=>{e.exports=null},109:(e,t,n)=>{"use strict";var r=n(867),o=["age","authorization","content-length","content-type","etag","expires","from","host","if-modified-since","if-unmodified-since","last-modified","location","max-forwards","proxy-authorization","referer","retry-after","user-agent"];e.exports=function(e){var t,n,s,i={};return e?(r.forEach(e.split("\n"),(function(e){if(s=e.indexOf(":"),t=r.trim(e.substr(0,s)).toLowerCase(),n=r.trim(e.substr(s+1)),t){if(i[t]&&o.indexOf(t)>=0)return;i[t]="set-cookie"===t?(i[t]?i[t]:[]).concat([n]):i[t]?i[t]+", "+n:n}})),i):i}},205:e=>{"use strict";e.exports=function(e){var t=/^([-+\w]{1,25})(:?\/\/|:)/.exec(e);return t&&t[1]||""}},713:e=>{"use strict";e.exports=function(e){return function(t){return e.apply(null,t)}}},675:(e,t,n)=>{"use strict";var r=n(867);e.exports=function(e,t){t=t||new FormData;var n=[];function o(e){return null===e?"":r.isDate(e)?e.toISOString():r.isArrayBuffer(e)||r.isTypedArray(e)?"function"==typeof Blob?new Blob([e]):Buffer.from(e):e}return function e(s,i){if(r.isPlainObject(s)||r.isArray(s)){if(-1!==n.indexOf(s))throw Error("Circular reference detected in "+i);n.push(s),r.forEach(s,(function(n,s){if(!r.isUndefined(n)){var a,c=i?i+"."+s:s;if(n&&!i&&"object"==typeof n)if(r.endsWith(s,"{}"))n=JSON.stringify(n);else if(r.endsWith(s,"[]")&&(a=r.toArray(n)))return void a.forEach((function(e){!r.isUndefined(e)&&t.append(c,o(e))}));e(n,c)}})),n.pop()}else t.append(i,o(s))}(e),t}},875:(e,t,n)=>{"use strict";var r=n(288).version,o=n(648),s={};["object","boolean","number","function","string","symbol"].forEach((function(e,t){s[e]=function(n){return typeof n===e||"a"+(t<1?"n ":" ")+e}}));var i={};s.transitional=function(e,t,n){function s(e,t){return"[Axios v"+r+"] Transitional option '"+e+"'"+t+(n?". "+n:"")}return function(n,r,a){if(!1===e)throw new o(s(r," has been removed"+(t?" in "+t:"")),o.ERR_DEPRECATED);return t&&!i[r]&&(i[r]=!0,console.warn(s(r," has been deprecated since v"+t+" and will be removed in the near future"))),!e||e(n,r,a)}},e.exports={assertOptions:function(e,t,n){if("object"!=typeof e)throw new o("options must be an object",o.ERR_BAD_OPTION_VALUE);for(var r=Object.keys(e),s=r.length;s-- >0;){var i=r[s],a=t[i];if(a){var c=e[i],u=void 0===c||a(c,i,e);if(!0!==u)throw new o("option "+i+" must be "+u,o.ERR_BAD_OPTION_VALUE)}else if(!0!==n)throw new o("Unknown option "+i,o.ERR_BAD_OPTION)}},validators:s}},867:(e,t,n)=>{"use strict";var r,o=n(849),s=Object.prototype.toString,i=(r=Object.create(null),function(e){var t=s.call(e);return r[t]||(r[t]=t.slice(8,-1).toLowerCase())});function a(e){return e=e.toLowerCase(),function(t){return i(t)===e}}function c(e){return Array.isArray(e)}function u(e){return void 0===e}var l=a("ArrayBuffer");function f(e){return null!==e&&"object"==typeof e}function d(e){if("object"!==i(e))return!1;var t=Object.getPrototypeOf(e);return null===t||t===Object.prototype}var p=a("Date"),h=a("File"),m=a("Blob"),g=a("FileList");function v(e){return"[object Function]"===s.call(e)}var y=a("URLSearchParams");function E(e,t){if(null!=e)if("object"!=typeof e&&(e=[e]),c(e))for(var n=0,r=e.length;n<r;n++)t.call(null,e[n],n,e);else for(var o in e)Object.prototype.hasOwnProperty.call(e,o)&&t.call(null,e[o],o,e)}var w,b=(w="undefined"!=typeof Uint8Array&&Object.getPrototypeOf(Uint8Array),function(e){return w&&e instanceof w});e.exports={isArray:c,isArrayBuffer:l,isBuffer:function(e){return null!==e&&!u(e)&&null!==e.constructor&&!u(e.constructor)&&"function"==typeof e.constructor.isBuffer&&e.constructor.isBuffer(e)},isFormData:function(e){var t="[object FormData]";return e&&("function"==typeof FormData&&e instanceof FormData||s.call(e)===t||v(e.toString)&&e.toString()===t)},isArrayBufferView:function(e){return"undefined"!=typeof ArrayBuffer&&ArrayBuffer.isView?ArrayBuffer.isView(e):e&&e.buffer&&l(e.buffer)},isString:function(e){return"string"==typeof e},isNumber:function(e){return"number"==typeof e},isObject:f,isPlainObject:d,isUndefined:u,isDate:p,isFile:h,isBlob:m,isFunction:v,isStream:function(e){return f(e)&&v(e.pipe)},isURLSearchParams:y,isStandardBrowserEnv:function(){return("undefined"==typeof navigator||"ReactNative"!==navigator.product&&"NativeScript"!==navigator.product&&"NS"!==navigator.product)&&"undefined"!=typeof window&&"undefined"!=typeof document},forEach:E,merge:function e(){var t={};function n(n,r){d(t[r])&&d(n)?t[r]=e(t[r],n):d(n)?t[r]=e({},n):c(n)?t[r]=n.slice():t[r]=n}for(var r=0,o=arguments.length;r<o;r++)E(arguments[r],n);return t},extend:function(e,t,n){return E(t,(function(t,r){e[r]=n&&"function"==typeof t?o(t,n):t})),e},trim:function(e){return e.trim?e.trim():e.replace(/^\s+|\s+$/g,"")},stripBOM:function(e){return 65279===e.charCodeAt(0)&&(e=e.slice(1)),e},inherits:function(e,t,n,r){e.prototype=Object.create(t.prototype,r),e.prototype.constructor=e,n&&Object.assign(e.prototype,n)},toFlatObject:function(e,t,n){var r,o,s,i={};t=t||{};do{for(o=(r=Object.getOwnPropertyNames(e)).length;o-- >0;)i[s=r[o]]||(t[s]=e[s],i[s]=!0);e=Object.getPrototypeOf(e)}while(e&&(!n||n(e,t))&&e!==Object.prototype);return t},kindOf:i,kindOfTest:a,endsWith:function(e,t,n){e=String(e),(void 0===n||n>e.length)&&(n=e.length),n-=t.length;var r=e.indexOf(t,n);return-1!==r&&r===n},toArray:function(e){if(!e)return null;var t=e.length;if(u(t))return null;for(var n=new Array(t);t-- >0;)n[t]=e[t];return n},isTypedArray:b,isFileList:g}}},t={};function n(r){var o=t[r];if(void 0!==o)return o.exports;var s=t[r]={exports:{}};return e[r](s,s.exports,n),s.exports}(()=>{"use strict";const e="SessionKey",t=async()=>new Promise(((t,n)=>{chrome.storage.local.get([e],(function(e){void 0===e.SessionKey?t(null):t(e.SessionKey)}))}));var r=chrome.runtime.connectNative("com.allianz.usercertificateautoenrollment");r.onDisconnect.addListener((function(){console.log(chrome.runtime.lastError),console.log("disconnected from native app."),r=null}));var o=-1;const s=[{Id:1,Description:"Command to get local certificates"},{Id:2,Description:"Send this with the PFX file to update certificates"},{Id:3,Description:"Send this with a ThumbprintID to make a certificate as auth certificate"},{Id:4,Description:"Get current domain user"},{Id:5,Description:"Get session logs"}];async function i(e,n){o=e;var r=await t(),s={CommandId:e,CommandValue:JSON.stringify(n),SessionKey:r};return console.log("3. Sending message to native app: "+JSON.stringify(s)),new Promise(((e,t)=>{chrome.runtime.sendNativeMessage("com.allianz.usercertificateautoenrollment",s,(async function(t){console.log("4. Response triggerd...");var n=await async function(e){switch(console.log("5. Recieved message from native app: "+JSON.stringify(e)),console.log("6. Command before "+o),o){case 1:case 3:break;case 2:console.log(e);break;case 4:await async function(e){await(async e=>new Promise(((t,n)=>{var r={};r.LoggedInUser=e,chrome.storage.local.set(r,(function(){console.log("7. Saving logged in user "+r.LoggedInUser),t(!0)}))})))(e.data)}(e);break;case 5:await async function(e){console.log(e)}(e)}return o=-1,!0}(t);e(n)}))}))}n(669);const a=$("#btnAuthorize"),c=$("#btnSyncCertificates");a.click((async function(){var e;console.log("1.Getting user from OS"),await(async()=>{var e=s[3];console.log("2. Sending command "+e.Id),await i(e.Id),console.log("8. "+o)})(),console.log("9. User retrieved..., authorizing"),(async()=>{var e=await(async()=>new Promise(((e,t)=>{chrome.storage.local.get(["LoggedInUser"],(function(t){void 0===t.LoggedInUser?e(null):e(t.LoggedInUser)}))})))();console.log("9. Domain"+e),(async e=>{new Promise(((t,n)=>{var r={};r.SessionKey=e,chrome.storage.local.set(r,(function(){console.log("Saving session key "+r.SessionKey),t(!0)}))}))})(function(e){for(var t="",n="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",r=n.length,o=0;o<10;o++)t+=n.charAt(Math.floor(Math.random()*r));return t}());var n=await t();console.log("11. Session key "+n)})(),console.log("User authorized"),e="Sucess!",$("#alert-strong-message").html(e),$("#alert-message").html("Authorize succeded!"),$(".alert").alert(),a.removeClass("d-block").addClass("d-none"),c.removeClass("d-none").addClass("d-block"),c.style.display="block"})),c.click((function(){(async()=>{console.log("1. Syncing certifcates"),console.log("2. Retriving local certicates");var e=await(async()=>{var e=s[0];console.log("Sending commnad "+e.Id),await i(e.Id)})();console.log("3. Local certificates =================="),console.log(e),console.log("4. ==================================="),console.log("5. Uploading certificates to server....");var n=await(async e=>(t(),await(async()=>({ImportCertificates:[{sn:"caa21",issuer:"CN=Allianz UserCA V, O=Allianz, C=DE",tp:"2F6873B5D3D962CBB1003D3AC79C997C5118ECFF",dn:"C=DE,O=Allianz,E=volker.zeihs@allianz.de,CN=Volker Zeihs",auth:!0},{sn:"098149736644D339",issuer:"CN=Allianz UserCA, O=Allianz, C=DE",tp:"F261488F38B4D49BB1003D3AC79C997C51181234",dn:"C=DE,O=Allianz,E=volker.zeihs@allianz.de,CN=Volker Zeihs",auth:!1}],asd:"yesss"}))()))();console.log("6. Certificates uploaded successfully"),console.log("7. Certificates that need sync"),console.log(n),console.log("7. Sending new certs to client");var r=await(async e=>{var t=s[1];console.log("Sending commnand "+t.Id),await i(t.Id,e)})(n);console.log("8.Certs sent successfully to client"),console.log(r),console.log("9. Client response")})(),btnGetLogs.style.display="block"})),(async()=>{(async e=>{new Promise(((e,t)=>{chrome.storage.local.remove(["SessionKey"],(function(t){console.log(t),e(!0)}))}))})()})()})()})();