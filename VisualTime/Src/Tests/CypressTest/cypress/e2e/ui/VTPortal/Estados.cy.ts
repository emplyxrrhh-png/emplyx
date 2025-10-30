


/// https://vtportalidi.azurewebsites.net/2/indexv2.aspx#status

//import { values } from "cypress/types/lodash";
import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"

const FIXTURES_SCREENSHOTS_FOLDER = "files/dom/"

describe('Portal-', () => {

  beforeEach(() => {
    loginHelper.loginPortal('g.loco', '12345678As+')
  });

  afterEach(() => {
    loginHelper.logoutPortal()
  })

  it('Estados vacaciones Solo titulo del elemento', baseHelper.retriesDefault, () => {
    cy.window().then((win) => {
      win.VTPortal.app.navigate("status", { root: true });


      cy.wait(1000 * 3)


      cy.get('.listMenuItemContentAcc > [data-bind="text: $data.Name"]').then(($element) => {
        const elementHtml = $element.text()
        cy.log(elementHtml)
        expect(elementHtml).to.equal('Vacaciones pendientes')
      })

    })
  })
  it('Vacaciones, se detecta que está la sección (DOM)', baseHelper.retriesDefault, () => {
    cy.window().then((win) => {
      win.VTPortal.app.navigate("status", { root: true });

    });
    //cy.pause()
    //cy.intercept('POST', 'https://vtportalidi.azurewebsites.net/api/portalsvcx.svc/GetAccrualsSummary').as('GetAccrualsSummary');

    cy.wait(1000 * 5)
    //cy.get("@GetAccrualsSummary").its('response.statusCode').should('eq', 200);

    //   cy.get('[data-options="dxViewPlaceholder: { viewName: \'accrualsHome\' } "] > .dx-view')
    const domPathFile = `${FIXTURES_SCREENSHOTS_FOLDER}employeeProfileStatus.html`
          /*  cy.get('[data-options="dxViewPlaceholder: { viewName: \'accrualsHome\' } "] > .dx-view').then(($element) => {
                        const elementHtml = $element[0].outerHTML
                        cy.writeFile('cypress/fixtures/files/dom/employeeProfileStatus.html', elementHtml)
                    })  
 */


    cy.fixture(domPathFile).then((savedHtml) => {

      cy.get('[data-options="dxViewPlaceholder: { viewName: \'accrualsHome\' } "] > .dx-view').then(($element) => {
        const currentHtml = $element[0].outerHTML
        const normalizedSavedHtml = baseHelper.normalizeHtml(savedHtml)
        const normalizedCurrentHtml = baseHelper.normalizeHtml(currentHtml)
        expect(normalizedCurrentHtml).to.equal(normalizedSavedHtml)
      })
    })
    // cy.get("#tabMain > div.dx-menu-adaptive-mode > div.dx-menu-hamburger-button.dx-button.dx-button-normal.dx-widget.dx-button-has-icon").click()

    //cy.wait(1000*3)
    //cy.get('li[data-item-id="14"]').click()

  })


})

