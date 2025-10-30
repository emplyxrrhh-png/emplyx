let editReportDescription;
(function() {
	let currentReportData;
	let editedReportData;

	editReportDescription = async () => {
		const isLoaded = await loadCurrentReport();
		if (!isLoaded) return;

		const template = await getViewTemplate("editReportDescription");
		const editorNode = document.createElement("div");
		editorNode.setAttribute("id", "fieldsEditor");
		editorNode.innerHTML = template;
		document.querySelector("#divReportDesigner").appendChild(editorNode);
		editorNode.style.display = "block";

		const textarea = document.querySelector("textarea#descriptionEdition");
		textarea.value = editedReportData.Description;
		textarea.addEventListener("change", () => {
			editedReportData.Description = textarea.value;
		});

		editorNode
			.querySelector(".editBtn > span#acceptEdition")
			.addEventListener("click", acceptEdition);
		editorNode
			.querySelector(".editBtn > span#cancelEdition")
			.addEventListener("click", cancelEdition);
	};

	const acceptEdition = async () => {
		const isSavedOK = await saveReportInfo(editedReportData);

		if (isSavedOK) {
			document.querySelector("#fieldsEditor").remove();
		} else {
			DevExpress.ui.dialog.alert(
				"algo ha ido mal al intentar guardar los datos",
				"Guardando Descripción"
			);
		}
	};

	const cancelEdition = async () => {
		document.querySelector("#fieldsEditor").remove();
	};

	const getViewTemplate = async (templateName = "editReportDescription") => {
		let template;
		await $.ajax({
			url: `${BASE_URL}Report/GetViewTemplate`,
			data: { "templateName": templateName, idReport: 0 },
			type: "POST",
			dataType: "text",
			success: data => (template = data),
			error: error => console.error(error)
		});

		return template;
	};

	const loadCurrentReport = async () => {
		const src = window.parent.document
			.querySelector("#ifPrincipal")
			.getAttribute("src");
		const reportId = src.substring(src.lastIndexOf("/") + 1);
		//console.log(reportId);

		if (!reportId) {
			DevExpress.ui.dialog.alert(
				"Solo se puede editar la descripción una vez creado el informe",
				"Editar Descripción"
			);
			return false;
		}

		await $.ajax({
			url: "/Report/GetReportByIdAsJson",
			data: { reportId },
			type: "POST",
			dataType: "json",
			success: data => (currentReportData = data),
			error: error => console.error(error)
		});
		//console.log(currentReportData);
		editedReportData = { ...currentReportData };
		window.currentReportData = currentReportData;

		return true;
	};

	const saveReportInfo = async (reportObj, flag = "report") => {
		const reportData = JSON.stringify(reportObj);
		let saved;

		await $.ajax({
			url: "/Report/SaveReportInfo",
			data: { reportData, flag },
			type: "POST",
			dataType: "text",
			success: data => (saved = data),
			error: error => console.error(error)
		});

		return saved === "True";
	};
})();
