(function () {
	const CARD_PARENT_SELECTOR = "#CardView_DXMainTable > tbody > tr";
	let $Cards;
	let $customSearchBar;
	let $categoryFilterSearch;

	$(document).ready(async function () {
		$customSearchBar = document.querySelector("#customSearchBar");
		$categoryFilterSearch = '';

		window.reloadCardsPanel = reloadCardsPanel;
		window.unselectCards = unselectCards;
		window.selectCardById = selectCardById;
		window.refreshCardTree = refreshCardTree;//() => handleSearch("");
		window.hideTree = hideTree;
		window.handleCardTreeCustomSearch = handleSearch;
		window.cardViewRefreshCallback = null;
		window.completeRefreshCallback = null;
		/*---------------------------LISTENERS LISTENERS LISTENERS LISTENERS ------------------------*/
		bindCardsClickEvent();
		$($customSearchBar).on("keyup", () => handleSearch(""));
		/*---------------------------LISTENERS LISTENERS LISTENERS LISTENERS ------------------------*/

		//$communiqueCards[0].click();
		reloadCardsPanel();
	});

	function bindCardsClickEvent() {
		$Cards = document.querySelectorAll(CARD_PARENT_SELECTOR);
		$($Cards).off("click", handleCardClick);
		$($Cards).on("click", handleCardClick);
    }

	function handleCardClick(thisCard) {
		//reset styles
		[...document.querySelectorAll(".CardsTree-card")].map((card) => {
			card.classList.remove("CardsTree-cardClicked");
		});

		//apply new styles
		let cardElement = thisCard.currentTarget
			.querySelector(".CardsTree-card");
		if (cardElement != null) cardElement.classList.add("CardsTree-cardClicked");
	}

	function unselectCards() {
		//reset styles
		[...document.querySelectorAll(".CardsTree-card")].map((card) => {
			card.classList.remove("CardsTree-cardClicked");
		});
	}

	function reloadCardsPanel() {
		const panel = document.querySelector(".dxcvCSD");
		resizePanel(panel);

		const $Cards = [...document.querySelectorAll(".CardsTree-card")];
		const cardsPanelObserver = buildCardsPanelObserver(panel);

		if ($Cards.length > 0) {
			cardsPanelObserver.observe($Cards[$Cards.length - 1]);
		}

		bindCardsClickEvent();


		if (window.cardViewRefreshCallback != null) {
			window.cardViewRefreshCallback();
			window.cardViewRefreshCallback = null;
        }
	}

	function handleSearch(forceReload = "", externFilter) {

		if (typeof externFilter != 'undefined') $categoryFilterSearch = externFilter;

		$originSearch = document.querySelector("#CardView_DXSE_I");
		$originSearch.value =
			$originSearch.value === `${$customSearchBar.value}${forceReload}`
			? `${$categoryFilterSearch} ${$customSearchBar.value}`
			: `${$categoryFilterSearch} ${$customSearchBar.value}${forceReload}`;

		$originSearch.onchange();

		reloadCardsPanel();
	}

	async function refreshCardTree(selectId = undefined) {
		let response = {};

		if (typeof selectId != 'undefined') {

			if (selectId == -2) {
				if ($Cards.length > 0) selectId = $Cards[0].querySelector(".cardsTree-CardInfo").getAttribute("data-card-id");
				else selectId = -1;
			}

			window.cardViewRefreshCallback = () => selectCardById(selectId);
		}

		try {
			CardView.Refresh();

			setTimeout(function () {			
				if (window.completeRefreshCallback != null) {
					window.completeRefreshCallback();
				}
			}, 300);

		} catch (e) { }

	}

	window.onresize = () => {
		resizePanel(document.querySelector(".dxcvCSD"));
	};

	function selectCardById(selectId) {

		if(parseInt(selectId,10) == -1) return;

		const cardIds = [...$Cards].map((c) => {
			const card = c.querySelector(".cardsTree-CardInfo");
			return card && card.getAttribute("data-card-id");
		});

		if (selectId !== undefined && selectId && cardIds.indexOf("" + selectId) > -1) {
			document.querySelector(
				`.CardsTree-card .cardsTree-CardInfo[data-card-id='${selectId}']`
			).parentElement.click();
		} else {
			let card = $Cards[0].querySelector(
				`.CardsTree-card .cardsTree-CardInfo`
			);
			card && card.parentElement.click();
		}
	}

	function hideTree() {
		let treeIsVisible = ! $("#divTree").is(":visible");

		$("#divTree").animate({ width: 'toggle', duration: 100 });

		treeIsVisible ? $("#unhideTreeBtnWrapper").fadeOut() : $("#unhideTreeBtnWrapper").fadeIn();
	}

	function resizePanel(panel) {
		panel.parentNode.style.height = `${window.innerHeight - 250}px`;
	}

	function buildCardsPanelObserver(container) {
		const transparentLayer = document.createElement("div");
		const parent = container.parentNode;

		transparentLayer.classList.add("scrollPanelLayer");
		parent.appendChild(transparentLayer);
		parent.style.verticalAlign = "top";
		parent.style.position = "relative";

		return new IntersectionObserver(
			(entries) => {
				if (entries[0].isIntersecting && entries[0].intersectionRatio >= 0.5) {
					transparentLayer.style.opacity = 0;
				} else {
					transparentLayer.style.opacity = 1;
				}
			},
			{ root: container, threshold: [0, 0.5, 1] }
		);
	}
})();
