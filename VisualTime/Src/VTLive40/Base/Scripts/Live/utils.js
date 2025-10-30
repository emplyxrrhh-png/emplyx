//*******************************************************************************************
// ** Author: I.Santaularia
// ** Date: 01/04/2025
// ** Description: Javascript tree state utils
//*******************************************************************************************

// ==========================================
// SECTION: Language utils
// ==========================================
//#region Language utils
function getTextFromCatalog(catalog, key, namespace) {
    let keyFound = true;

    if (!key || key.length == 0) keyFound = false;

    let catalogKey = key;
    if (typeof catalog !== 'undefined') {
        if (typeof namespace !== 'undefined' && namespace.length > 0) catalogKey = `${namespace}#${key}`;
        if (typeof catalog[catalogKey] === 'undefined') keyFound = false;
    } else {
        keyFound = false;
    }

    if (keyFound) return catalog[catalogKey];
    else {
        try {
            return Globalize.formatMessage(key);
        } catch {
            console.error(`Key ${catalogKey} not found on catalog and key ${key} not found on json resources.`);
        }
    }

    return key;
}
//#endregion

// ==========================================
// SECTION: Date utils
// ==========================================
//#region Date utils
function getDateLocalizationFormats() {
    let locale = {
        aspLang: '',
        language: '',
        format: '',
        separator: '',
        moment: ''
    };

    let cookie_locale = readCookie("VTLive_Language", "es");

    switch (cookie_locale) {
        case "es":
            locale.aspLang = 'ESP';
            locale.language = 'es';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' hasta ';
            break;
        case "ca":
            locale.aspLang = 'CAT';
            locale.language = 'ca';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' fins ';
            break;
        case "pt":
            locale.aspLang = 'POR';
            locale.language = 'pt';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' até ';
            break;
        case "gl":
            locale.aspLang = 'GAL';
            locale.language = 'gl';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' ata ';
            break;
        case "it":
            locale.aspLang = 'ITA';
            locale.language = 'it';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' fino a ';
            break;
        case "fr":
            locale.aspLang = 'FRA';
            locale.language = 'fr';
            locale.format = 'dd/MM/yyyy';
            locale.moment = 'DD/MM/YYYY';
            locale.separator = ' au ';
            break;
        case "en":
        default:
            locale.language = 'ENG';
            locale.language = 'en';
            locale.format = 'M-dd-yyyy';
            locale.moment = 'MM-DD-YYYY';
            locale.separator = ' to ';
            break;
    }

    return locale;
}

function dateToMsJson(date) {
    // Asegurarse de que date sea un objeto Date
    if (!(date instanceof Date)) date = new Date(date);

    // Calcular ticks
    const millisecondsSinceUnixEpoch = date.getTime();

    // Devolver como string (los ticks suelen ser números muy grandes)
    return `/Date(${millisecondsSinceUnixEpoch.toString()})/`;
}

function dateToJsonIsoString(localDate) {

    const offsetMs = localDate.getTimezoneOffset();

    let dateWithOffset = moment(localDate);
    dateWithOffset.add(offsetMs, 'minutes');

    localDate = dateWithOffset.toDate();

    const year = localDate.getFullYear();
    const month = localDate.getMonth();        // ¡Ojo! Los meses van de 0 a 11
    const day = localDate.getDate();
    const hours = localDate.getHours();
    const minutes = localDate.getMinutes();
    const seconds = localDate.getSeconds();

    const utcDate = new Date(Date.UTC(year, month, day, hours, minutes, seconds));


    return utcDate.toISOString();
}


//#endregion

// ==========================================
// SECTION: Selector de contexto
// ==========================================
//#region Context selector
let trackedEntityChanges = {};
function generateDefaultFilter(customView) {
    let tmpView = { Employees: [], Groups: [], Collectives: [], LabAgrees: [], Operation: "or", Filter: "", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false };

    if (customView == 'All' || customView == 'None') {
        tmpView.ComposeMode = customView;
    } else {
        tmpView.ComposeMode = "Custom";
        tmpView.ComposeFilter = customView;
        customView.split(',').forEach(function (element) {
            if (element.charAt(0) == 'A') {
                tmpView.Groups.push({ IdGroup: parseInt(element.substring(1), 10), Name: '' });
            } else if (element.charAt(0) == 'B') {
                tmpView.Employees.push({ IdEmployee: parseInt(element.substring(1), 10), EmployeeName: '' });
            } else if (element.charAt(0) == 'C') {
                tmpView.Collectives.push({ FieldValue: parseInt(element.substring(1), 10), FieldName: '' });
            } else if (element.charAt(0) == 'L') {
                tmpView.LabAgrees.push({ FieldValue: parseInt(element.substring(1), 10), FieldName: '' });
            }
        });
    }

    return tmpView;
}

function fillComponentsData(customView) {
    let tmpView = customView;

    if (typeof tmpView["Employees"] == 'undefined') tmpView["Employees"] = [];
    if (typeof tmpView["Groups"] == 'undefined') tmpView["Groups"] = [];
    if (typeof tmpView["Collectives"] == 'undefined') tmpView["Collectives"] = [];
    if (typeof tmpView["LabAgrees"] == 'undefined') tmpView["LabAgrees"] = [];
    if (typeof tmpView["ComposeMode"] == 'undefined') tmpView["ComposeMode"] = "Custom";
    if (typeof tmpView["ComposeFilter"] == 'undefined') tmpView["ComposeFilter"] = "Custom";
    if (typeof tmpView["Filters"] != 'undefined') delete tmpView.Filters;

    tmpView.Groups = [];
    tmpView.Employees = [];
    tmpView.Collectives = [];
    tmpView.LabAgrees = [];

    if (tmpView.ComposeMode != 'All' && tmpView.ComposeMode != 'None') {
        tmpView.ComposeFilter.split(',').forEach(function (element) {
            if (element.charAt(0) == 'A') {
                tmpView.Groups.push({ IdGroup: parseInt(element.substring(1), 10), Name: '' });
            } else if (element.charAt(0) == 'B') {
                tmpView.Employees.push({ IdEmployee: parseInt(element.substring(1), 10), EmployeeName: '' });
            } else if (element.charAt(0) == 'C') {
                tmpView.Collectives.push({ FieldValue: parseInt(element.substring(1), 10), FieldName: '' });
            } else if (element.charAt(0) == 'L') {
                tmpView.LabAgrees.push({ FieldValue: parseInt(element.substring(1), 10), FieldName: '' });
            }
        });
    }

    return tmpView;
}

function buildSelectedEmployeesString(currentView) {

    let tmpView = null;

    if (typeof currentView == 'object') {
        tmpView = currentView;
    } else {
        tmpView = generateDefaultFilter(currentView);
    }

    tmpView = fillComponentsData(tmpView);

    let selectionDescription = "";
    try {

        if (typeof tmpView.ComposeMode != 'undefined' && (tmpView.ComposeMode == 'All' || tmpView.ComposeMode == 'None')) {
            selectionDescription = Globalize.formatMessage(`roSelectorEmployees_Mode_${tmpView.ComposeMode}`)
        } else {
            if ((tmpView.Employees.length + tmpView.Groups.length + tmpView.Collectives.length + tmpView.LabAgrees.length) == 1) {
                if (tmpView.Employees.length > 0) {
                    if (typeof tmpView.Employees[0].EmployeeName == 'undefined' || tmpView.Employees[0].EmployeeName == '') selectionDescription = `1 ${Globalize.formatMessage('roSelectorEmployee')}`;
                    else selectionDescription = `${tmpView.Employees[0].EmployeeName}`;
                }
                if (tmpView.Groups.length > 0) {
                    if (typeof tmpView.Groups[0].Name == 'undefined' ||tmpView.Groups[0].Name == '') selectionDescription = `1 ${Globalize.formatMessage('roSelectorGroup')}`;
                    else selectionDescription = `${tmpView.Groups[0].Name}`;
                }
                if (tmpView.Collectives.length > 0) {
                    if (typeof tmpView.Collectives[0].FieldName == 'undefined' ||tmpView.Collectives[0].FieldName == '') selectionDescription = `1 ${Globalize.formatMessage('roSelectorCollective')}`;
                    else selectionDescription = `${tmpView.Collectives[0].FieldName}`;
                }
                if (tmpView.LabAgrees.length > 0) {
                    if (typeof tmpView.LabAgrees[0].FieldName == 'undefined' || tmpView.LabAgrees[0].FieldName == '') selectionDescription = `1 ${Globalize.formatMessage('roSelectorLabAgree')}`;
                    else selectionDescription = `${tmpView.LabAgrees[0].FieldName}`;
                }
            } else {
                let andString = ' ' + Globalize.formatMessage('roSelectorAND') + ' ';
                let groupsSeparator = ((tmpView.Collectives.length + tmpView.LabAgrees.length) > 0 ? ', ' : andString);
                let collectivesSeparator = ((tmpView.LabAgrees.length) > 0 ? ', ' : andString);

                if (tmpView.Employees.length > 0) selectionDescription += `${tmpView.Employees.length} ${tmpView.Employees.length > 1 ? Globalize.formatMessage('roSelectorEmployees') : Globalize.formatMessage('roSelectorEmployee')}`;
                if (tmpView.Groups.length > 0) selectionDescription += `${selectionDescription.length > 0 ? groupsSeparator : ''}${tmpView.Groups.length} ${tmpView.Groups.length > 1 ? Globalize.formatMessage('roSelectorGroups') : Globalize.formatMessage('roSelectorGroup')}`;
                if (tmpView.Collectives.length > 0) selectionDescription += `${selectionDescription.length > 0 ? collectivesSeparator : ''}${tmpView.Collectives.length} ${tmpView.Collectives.length > 1 ? Globalize.formatMessage('roSelectorCollectives') : Globalize.formatMessage('roSelectorCollective')}`;
                if (tmpView.LabAgrees.length > 0) selectionDescription += `${selectionDescription.length > 0 ? andString : ''}${tmpView.LabAgrees.length} ${tmpView.LabAgrees.length > 1 ? Globalize.formatMessage('roSelectorLabAgrees') : Globalize.formatMessage('roSelectorLabAgree')}`;
            }

            if (selectionDescription == '') selectionDescription = Globalize.formatMessage('roSelectorEmpty');
        }


    } catch (e) {
        selectionDescription = e;
    }


    return selectionDescription;
}

async function loadSelectorViewFromServer(serverURL, universalSelectorName, pageName) {
    let tmpView = null;

    let response = await fetch(serverURL + "/GetUniversalSelector", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ sSelectorName: universalSelectorName })
    });

    // Verificar la respuesta
    if (!response.ok) {
        tmpView = generateDefaultFilter(undefined);
    } else {
        let sResponse = await response.json();

        if (sResponse != null && sResponse != "") {
            tmpView = JSON.parse(sResponse);

            let actualTicks = Date.now();
            localStorage.setItem(pageName, JSON.stringify({ view: tmpView, timestamp: actualTicks }));
        }
    }

    return tmpView;
}
//#endregion

// ==========================================
// SECTION: Save changes bar
// ==========================================
//#region Save changes bar

async function deleteEntity(e) {
    let section = e.element.attr("section");

    DevExpress.ui.dialog.confirm(
        (viewUtilsManager.DXTranslate(`roDeleteConfirmQuestion_${section}`).length > 0) ? viewUtilsManager.DXTranslate(`roDeleteConfirmQuestion_${section}`) : viewUtilsManager.DXTranslate(`roDeleteConfirmQuestion_`),
        (viewUtilsManager.DXTranslate(`roDeleteConfirmOk_${section}`).length > 0) ? viewUtilsManager.DXTranslate(`roDeleteConfirmOk_${section}`) : viewUtilsManager.DXTranslate(`roDeleteConfirmOk_`)
    ).done( async function (result) {
        if (result) {

            const deleteFunc = `${section}_delete`; 

            if (typeof window[deleteFunc] === 'function') {
                let result = await window[deleteFunc](e);
                processResult(result, section);
            } else { 
                console.error(`Function ${deleteFunc} not found.`);
            }
        }
    });
}

async function visualizeEntity(e) {
    let section = e.element.attr("section");

    const visualizeFunc = `${section}_visualize`;

    if (typeof window[visualizeFunc] === 'function') {
        let result = await window[visualizeFunc](e);
        //processResult(result, section); //No modificamos el estado de los otros botones
    } else {
        console.error(`Function ${visualizeFunc} not found.`);
    }
}

async function undoEntity(e) {
    let section = e.element.attr("section");

    const undoFunc = `${section}_undo`;

    if (typeof window[undoFunc] === 'function') {
        let result = await window[undoFunc](e);
        processResult(result, section);
    } else {
        console.error(`Function ${undoFunc} not found.`);
    }
}

async function saveEntity(e) {
    let section = e.element.attr("section");

    const saveFunc = `${section}_save`;

    if (typeof window[saveFunc] === 'function') {
        let result = await window[saveFunc](e);
        processResult(result, section);
    } else {
        console.error(`Function ${saveFunc} not found.`);
    }
}

async function closeEntity(e) {
    let section = e.element.attr("section");

    const saveFunc = `${section}_close`;

    if (typeof window[saveFunc] === 'function') {
        let result = await window[saveFunc](e);
        processResult(result, section);
    } else {
        console.error(`Function ${saveFunc} not found.`);
    }
}

function processResult(data, section) {
    if (typeof data === "object" && data != null) {
        if (data.Success) {
            setEntityChanges(section, false);
            sb_hasChanges(section, false);
        }
    } else {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roConvertedError'), 'error', 2000); //TODO: AÑADIR UN TEXTO DE ERROR
    }
}

function sb_hasChanges(section, changed) {
    let currentEntityChanges = null;

    if (typeof trackedEntityChanges !== 'undefined' && typeof trackedEntityChanges[section] !== 'undefined') {
        currentEntityChanges = trackedEntityChanges[section];
    } else {
        currentEntityChanges = false;
    }

    if (changed) currentEntityChanges = true;

    if (typeof $(`#${section}_btnUndoEntity`).dxButton("instance") !== 'undefined') $(`#${section}_btnUndoEntity`).dxButton("instance").option("disabled", !currentEntityChanges);
    if (typeof $(`#${section}_btnSaveEntity`).dxButton("instance") !== 'undefined') $(`#${section}_btnSaveEntity`).dxButton("instance").option("disabled", !currentEntityChanges);
    if (typeof $(`#${section}_btnCloseEntity`).dxButton("instance") !== 'undefined') $(`#${section}_btnCloseEntity`).dxButton("instance").option("disabled", false);

    setEntityChanges(section, currentEntityChanges);
}

function setDeleteStatus(section, enabled) {
    if (typeof $(`#${section}_btnDeleteEntity`).dxButton("instance") !== 'undefined') $(`#${section}_btnDeleteEntity`).dxButton("instance").option("disabled", !enabled);
}

function sectionHasChanges(section) {
    if (typeof trackedEntityChanges === 'undefined') {
        trackedEntityChanges = {};
    }

    if (typeof trackedEntityChanges[section] === 'undefined') {
        return false;
    }

    return trackedEntityChanges[section];
}

function setEntityChanges(section, val) {
    if (typeof trackedEntityChanges === 'undefined') {
        trackedEntityChanges = {};
    }

    trackedEntityChanges[section] = val;
}

function setChangesBarTitle(section, title) {
    $(`#${section}_spanTitle`).text(title);
}
//#endregion

// ==========================================
// SECTION: Wait for js load
// ==========================================
//#region Wait for js load
window.functionWaitQueue = window.functionWaitQueue || {};

// Función para registrar cuando una función esté disponible
function registerFunction(functionName) {
    window.functionWaitQueue[functionName] = window.functionWaitQueue[functionName] || [];
    if (window.functionWaitQueue[functionName].length > 0) {
        // Ejecutar todas las funciones en espera
        window.functionWaitQueue[functionName].forEach(callback => callback());
        window.functionWaitQueue[functionName] = [];
    }
}

// Función para esperar a que una función esté disponible
function waitForFunction(functionName, callback) {
    if (typeof window[functionName] === 'function') {
        // La función ya existe
        callback();
    } else {
        // Agregar a la cola de espera
        window.functionWaitQueue[functionName] = window.functionWaitQueue[functionName] || [];
        window.functionWaitQueue[functionName].push(callback);
    }
}
//#endregion

/**
* Removes accents from a string.
* @param {string} str - The string to remove accents from.
* @returns {string} - The string without accents.
*/
function removeAccents(str) {
    if (typeof str !== 'string') return '';
    // Normalize to NFD Unicode form to separate accents from letters, then remove diacritics.
    // \u0300-\u036f is the range for combining diacritical marks.
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

/**
* Generates a short name from a full name with configurable options
* @param {string} fullName - The full name to convert to short name
* @param {string[]} existingShortNames - Array of existing short names to avoid duplicates
* @param {number} length - Desired length of short name (default: 3)
* @param {Object} options - Additional options for customization
* @returns {string} - Generated short name
*/
function generateShortName(fullName, existingShortNames = [], length = 3, options = {}) {
    // Default options
    const defaultOptions = {
        timeFormat: true, // Whether to handle time format (XX:XX - XX:XX) specially
        singleWordMode: 'prefix', // 'prefix' takes first n chars, 'initials' takes initials from the word
        fillMode: 'letters' // 'letters' uses more letters from words, 'numbers' adds numbers
    };

    const config = { ...defaultOptions, ...options };

    // Normalize fullName: remove accents and trim
    fullName = removeAccents(fullName || "").trim();
    let shortName = '';

    if (!fullName) return ''.padEnd(length, '1');

    const words = fullName.split(' ').filter(word => word.length > 0); // Filter out empty strings from multiple spaces

    // If fullName becomes empty after normalization and splitting (e.g., was only spaces or only accents)
    if (words.length === 0) return ''.padEnd(length, '1');

    // Detect time format: XX:XX - XX:XX text
    const timePattern = /\d{2}:\d{2}/;
    const hasTimeFormat = config.timeFormat &&
        words.length >= 3 &&
        timePattern.test(words[0]) &&
        words[1] === '-' &&
        timePattern.test(words[2]);

    if (hasTimeFormat) {
        // Prioritize text after the times
        for (let i = 3; i < words.length; i++) {
            if (words[i].length > 0) {
                shortName += words[i][0].toUpperCase();
            }
        }
    } else if (config.timeFormat && timePattern.test(words[0]) && words.length === 1) {
        // For single time values, convert to numeric
        shortName = words[0].replace(':', '');
    } else if (words.length > 1) {
        // Take first letter of first word, first letter of second word, and first letter of last word
        shortName += words[0][0].toUpperCase();
        if (words.length > 1 && shortName.length < length) { // Ensure second word exists and we need more chars
            shortName += words[1][0].toUpperCase();
        }

        // Add last word's initial if it's different from the second word and we need more chars
        if (words.length > 2 && shortName.length < length) {
            shortName += words[words.length - 1][0].toUpperCase();
        }
    } else { // Single word
        // Handle single word based on config
        if (config.singleWordMode === 'prefix') {
            shortName = words[0].substring(0, length).toUpperCase();
        } else { // 'initials' mode
            const word = words[0];
            shortName = word[0].toUpperCase();

            if (word.length >= 3 && shortName.length < length) {
                shortName += word[Math.floor(word.length / 2)].toUpperCase();
            }
            if (word.length >= 2 && shortName.length < length) { // Use last char if >=2 and still need more
                shortName += word[word.length - 1].toUpperCase();
            }
        }
    }

    // Truncate if longer than requested length
    if (shortName.length > length) {
        shortName = shortName.substring(0, length);
    }

    // Add more characters if shorter than requested length
    if (shortName.length < length) {
        if (config.fillMode === 'letters') {
            // Add more letters from words
            for (let i = 0; i < words.length && shortName.length < length; i++) {
                for (let j = (i === 0 && words.length > 1 && words[0].length > 0 && shortName.includes(words[0][0].toUpperCase())) ? 1 : 0; j < words[i].length && shortName.length < length; j++) {
                    // Avoid re-adding the very first letter if already taken, unless it's a single word or first word fill
                    if (shortName.length === 0 || !(i === 0 && j === 0 && words.length > 1 && shortName.startsWith(words[0][0].toUpperCase()))) {
                        // Check if the character is already part of the shortName from a previous step (e.g. initials from multi-word)
                        // This check is complex due to various ways initials are formed. A simpler approach is to just append and truncate later,
                        // but the original logic tries to be more selective.
                        // For now, we'll stick to appending subsequent characters.
                        if (j > 0 || (i > 0 && words[i].length > 0)) { // only add subsequent chars or from subsequent words
                            if (words[i][j]) shortName += words[i][j].toUpperCase();
                        } else if (i === 0 && words.length === 1 && j > 0) { // single word, add subsequent chars
                            if (words[i][j]) shortName += words[i][j].toUpperCase();
                        }
                    }
                }
            }
        }

        // If still too short, add numbers
        let index = 1;
        while (shortName.length < length) {
            shortName += index;
            index++;
        }
    }

    // Handle duplicates if existingShortNames is provided
    if (existingShortNames && existingShortNames.length > 0) {
        let index = 1;
        const originalShortName = shortName.substring(0, length); // Ensure original is also of correct length

        let tempShortName = originalShortName;
        while (existingShortNames.includes(tempShortName)) {
            const keepChars = Math.max(1, length - String(index).length);
            tempShortName = originalShortName.substring(0, keepChars) + index;
            // Ensure the generated name with number doesn't exceed length
            if (tempShortName.length > length) {
                tempShortName = tempShortName.substring(0, length);
            }
            index++;
            if (index > 1000) break; // Safety break
        }
        shortName = tempShortName;
    }

    // Ensure exact length as a final step
    return shortName.substring(0, length);
}