import cypress = require("cypress");
import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"

describe('Pantalla de Inicio-', baseHelper.retriesDefault, () => {
    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+')
    });

    afterEach(() => {
        loginHelper.logout()
    })
    //////////////////////////
    // 1. Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados.
    it('Verificacion de carga inicial', () => {

        cy.getIframeBody('#ifPrincipal').find('#buttonRequests');
        expect('#buttonRequests').to.exist

        cy.getIframeBody('#ifPrincipal').find('#divTree > div:nth-child(1) > div.panHeaderDashboardSmallGlobal').invoke('text').then((text) => {
            expect(text.trim()).to.equal("Solicitudes pendientes de mi equipo")
        })

    })
})