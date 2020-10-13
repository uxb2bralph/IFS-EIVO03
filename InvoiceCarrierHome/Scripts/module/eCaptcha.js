(function () {
	var module = angular.module("eCaptcha", ['compost']);

	module.component("eCaptcha", {
		templateUrl: '/APCONSUMER/COMMON/eCaptcha.html',
		bindings: {
			triggerRefresh: "<"
		},
		controller: function () {

			this.$onChanges = function (changes) {

				if (changes.triggerRefresh) {
					switch (changes.triggerRefresh.previousValue) {
						case true:
						case false:
							this.refresh(true);
							break;
						default:
							this.refresh(this.triggerRefresh);
							break;
					}
				}
			};

			refresh = function (init) {
				var img = $("[ecaptcha=captcha-image]").eq(0);
				var src = $(img).attr("data-src");
				var num = Date.now();
				if (init) {
					$(img).load(function () {
						$(img).unbind("load");

						refresh();
						$("[ecaptcha=captcha-audio]").each(function () {
							$(this).find("source").each(function () {
								$(this).attr('src', $(this).attr('data-src') + '?refresh=' + num);
							});
							$(this).load();
						});
					});
					$(img).attr("src", src + "?refresh=" + num);
				} else {
					for (var i = 0; i < $("[ecaptcha=captcha-image]").size(); i++) {
						$("[ecaptcha=captcha-image]").eq(i).attr("src", src + "?init=" + num);
					}
				}
			};

			this.refresh = refresh;

			this.play = function () {
				if (!(false || !!document.documentMode)) {
					$("[ecaptcha=captcha-audio]")[0].play();
				} else {
					var audioSrc = $("[ecaptcha=captcha-audio] > source").eq(0).attr('src');

					var oneTimeAudioPlayer = document.createElement('embed');
					oneTimeAudioPlayer.setAttribute('src', audioSrc);
					oneTimeAudioPlayer.setAttribute('hidden', 'true');
					oneTimeAudioPlayer.setAttribute('autoplay', 'true');
					document.body.appendChild(oneTimeAudioPlayer);

					setTimeout(function () {
						document.body.removeChild(oneTimeAudioPlayer);
					}, 30 * 1000)
				}
			};

			this.refresh(false);
		}
	});

})();