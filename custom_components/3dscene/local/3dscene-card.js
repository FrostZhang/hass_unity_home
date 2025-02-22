// 延时加载，解决每次界面显示不了的问题
; (() => {
  const timer = setInterval(() => {
	if(HTMLElement)
      clearInterval(timer);
      // 开始生成DOM元素
      class UnityCard extends HTMLElement { 

        set hass(hass) {
            this._hass = hass;
			this.update();
			
			if(!this.content){
			  this.lk = document.createElement("link");
			  this.lk.rel = "stylesheet";
			  this.lk.href = "/3dscene_local/TemplateData/style.css";
			  this.lk.type= "text/css";
			  const card = document.createElement("ha-card");
			  this.content = document.createElement("div");
			  this.content.id = "unity-container";
			  this.content.classList.add("unity-desktop");
			  this.content.innerHTML +=`
				  <canvas id="unity-canvas" width=960 height=600></canvas>
				  <div id="unity-loading-bar">
					  <div id="unity-logo"></div>
					  <div id="unity-progress-bar-empty">
						  <div id="unity-progress-bar-full"></div>
					  </div>
				  </div>
				  <!--  <div id="unity-fullscreen-button"></div> -->
				  <div id="unity-warning"> </div>
				  <div id="unity-footer">
					  <!-- <div id="unity-webgl-logo"></div> -->
					  
					  <!-- <div id="unity-build-title">HassHome</div> -->
				  </div>
			  `
			  card.appendChild(this.lk);
			  card.appendChild(this.content);
			  this.shadowRoot.appendChild(card);
			  this.test();
		    }
		}
		
        static get properties() {
          return {
            config: Object,
            asherlink3dscence: Object,
          };
        }

        constructor() {
          super();
		  this.attachShadow({ mode: 'open' });
		  console.info(`%c3dscene-card `, "color: green; font-weight: bold", "")
        }

        // 自定义默认配置
        static getStubConfig() {
          //return { entity: "weather.tian_qi" }
        }
		
        setConfig(config) {
		  var len = document.getElementsByTagName("scene3d-card").length;
		  if(len>0) throw "Only one instance is allowed";
          this.config = config;
        }

	    test() {
            var container = this.shadowRoot.querySelector("#unity-container");
            var canvas = this.shadowRoot.querySelector("#unity-canvas");
            var loadingBar = this.shadowRoot.querySelector("#unity-loading-bar");
            var progressBarFull = this.shadowRoot.querySelector("#unity-progress-bar-full");
            var warningBanner = this.shadowRoot.querySelector("#unity-warning");
            function unityShowBanner(msg, type) {
              function updateBannerVisibility() {
                warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
              }
              var div = document.createElement('div');
              div.innerHTML = msg;
              warningBanner.appendChild(div);
              if (type == 'error') div.style = 'background: red; padding: 10px;';
              else {
                if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
                setTimeout(function () {
                  warningBanner.removeChild(div);
                  updateBannerVisibility();
                }, 5000);
              }
              updateBannerVisibility();
            }
            var buildUrl = "/3dscene_local/Build";
            var loaderUrl = buildUrl + "/prize.loader.js";
            var config = {
              dataUrl: buildUrl + "/prize.data.unityweb",
              frameworkUrl: buildUrl + "/prize.framework.js.unityweb",
              codeUrl: buildUrl + "/prize.wasm.unityweb",
              streamingAssetsUrl: "3dscene_local/StreamingAssets",
              companyName: "AsherLink",
              productName: "scene3d-card",
              productVersion: "1.0.7",
              showBanner: unityShowBanner,
            };
            console.log(navigator.userAgent)
            if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
              container.className = "unity-mobile";
              // Avoid draining fillrate performance on mobile devices,
              // and default/override low DPI mode on mobile browsers.
              config.devicePixelRatio = 2;
              //unityShowBanner('');
            } else {
              container.className = "unity-mobile";
              config.devicePixelRatio = 1;
              //canvas.style.width = "960px";
              //canvas.style.height = "600px";
            }
            loadingBar.style.display = "block";
			
			var lv = document.getElementById("loadView");
			if	(lv!=null ) lv.remove();
            var loadView = document.createElement("script");
			loadView.id = "loadView"
            loadView.src = loaderUrl;
            loadView.onload = () => {
              createUnityInstance(canvas, config, (progress) => {
                progressBarFull.style.width = 100 * progress + "%";
              }).then((unityInstance) => {
                loadingBar.style.display = "none";
                this.asherlink3dscence = unityInstance;
				unityInstance.Module.SystemInfo.hassCard = this;
				unityInstance.Module.SystemInfo.hass = this._hass;
				console.log(this._hass.states)
            }).catch((message) => {
                alert(message);
              });
            };
            document.body.appendChild(loadView)
        }

        configChanged(newConfig) {
          const event = new Event("config-changed", {
            bubbles: true,
            composed: true
          });
          event.detail = { config: newConfig };
          this.dispatchEvent(event);
        }

        update() {
          //回传3d
          if (this.asherlink3dscence == undefined) return;
		  this.asherlink3dscence.Module.SystemInfo.hass = this._hass;
		  this.asherlink3dscence.SendMessage("Shijie", "HassUpdate",JSON.stringify(this._hass.states));
        }
		
        getCardSize() {
          return 4;
        }

        _fire(type, detail, options) {
          const node = this.shadowRoot;
          options = options || {};
          detail = (detail === null || detail === undefined) ? {} : detail;
          const e = new Event(type, {
            bubbles: options.bubbles === undefined ? true : options.bubbles,
            cancelable: Boolean(options.cancelable),
            composed: options.composed === undefined ? true : options.composed
          });
          e.detail = detail;
          node.dispatchEvent(e);
          return e;
        }
      }

      customElements.define('scene3d-card', UnityCard);

      // 添加预览
      window.customCards = window.customCards || [];
      window.customCards.push({
        type: "scene3d-card",
        name: "homeassistant 3D",
        preview: false,
        description: "Build your home -- AsherLink"
      });
	  
  }, 1000)
})();
 