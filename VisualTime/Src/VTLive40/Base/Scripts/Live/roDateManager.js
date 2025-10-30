// date-manager.js

(function (window, moment) {
    'use strict';

    // Verificamos que las dependencias (moment y moment-timezone) se hayan cargado primero.
    if (typeof moment === 'undefined' || typeof moment.tz === 'undefined') {
        throw new Error('DateManager requires moment.js and moment-timezone.js to be loaded first.');
    }

    class DateManager {
        #momentObj;
        timezone;
        constructor(inputDate, targetTimezone) {
            this.timezone = targetTimezone || moment.tz.guess();

            if (inputDate instanceof DateManager)
                this.#momentObj = moment(inputDate.toDate()).tz(this.timezone);
            else
                this.#momentObj = moment(inputDate).tz(this.timezone);


            if (!this.#momentObj.isValid()) {
                throw new Error('Fecha de entrada no válida.');
            }
        }

        add(amount, unit) {
            const newMoment = this.#momentObj.clone().add(amount, unit);
            return new DateManager(newMoment, this.timezone);
        }

        subtract(amount, unit) {
            const newMoment = this.#momentObj.clone().subtract(amount, unit);
            return new DateManager(newMoment, this.timezone);
        }

        startOf(unit) {
            const newMoment = this.#momentObj.clone().startOf(unit);
            return new DateManager(newMoment, this.timezone);
        }

        endOf(unit) {
            const newMoment = this.#momentObj.clone().endOf(unit);
            return new DateManager(newMoment, this.timezone);
        }

        format(formatStr = 'YYYY-MM-DD HH:mm:ss') {
            return this.#momentObj.format(formatStr);
        }

        fromNow() {
            return this.#momentObj.fromNow();
        }

        diff(otherDate, unit, float = false) {
            if (!(otherDate instanceof DateManager)) {
                throw new Error('El argumento debe ser una instancia de DateManager.');
            }
            return this.#momentObj.diff(otherDate.toMoment(), unit, float);
        }

        toDate() {
            return this.#momentObj.toDate();
        }

        toISOString(withLocalOffset) {
            return this.#momentObj.toISOString(withLocalOffset);
        }

        toMoment() {
            return this.#momentObj;
        }

        toTimezone(newTimezone, keepSameTime) {

            if (typeof keepSameTime !== 'undefined') {
                if (!moment.tz.zone(newTimezone)) {
                    console.warn(`Timezone "${newTimezone}" no es válida. No se realizó el cambio.`);
                    return this;
                }
                // El segundo argumento `true` (keepLocalTime) es la clave aquí.
                const newMoment = this.#momentObj.clone().tz(newTimezone, true);
                return new DateManager(newMoment, newTimezone);
            } else {
                if (!moment.tz.zone(newTimezone)) {
                    console.warn(`Timezone "${newTimezone}" no es válida. No se realizó la conversión.`);
                    return this;
                }
                const newMoment = this.#momentObj.clone().tz(newTimezone);
                return new DateManager(newMoment, newTimezone);
            }
        }

        toUTC(keepSameTime) {
            return this.toTimezone('UTC', keepSameTime);
        }

    }

    window.DateManager = DateManager;

})(window, window.moment); // Pasamos los objetos globales `window` y `moment` a nuestra IIFE.