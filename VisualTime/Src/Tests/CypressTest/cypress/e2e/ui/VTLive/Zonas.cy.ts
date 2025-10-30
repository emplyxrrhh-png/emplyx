import { method } from "cypress/types/lodash";
import * as loginHelper from "../../../helpers/loginHelper"


describe('Zona Pantalla-', () => {

    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+')
        cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Zones/GetZones?*').as('GetZones');
        cy.window().then((win) => {
            win.reenviaFrame('/.//Zones', '', '', '');
        });
        
        cy.wait('@GetZones').its('response.statusCode').should('eq', 200);
    });

    afterEach(() => {
        loginHelper.logout()
    })



    it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de Zona.', () => {


        cy.getIframeBody('#ifPrincipal').find("#main > div:nth-child(7) > div > div > div:nth-child(1) > div:nth-child(2) > div:nth-child(1)").invoke('text').then((text) => {
            expect(text.trim()).to.equal("Zonas".trim())
        })
    })

})
