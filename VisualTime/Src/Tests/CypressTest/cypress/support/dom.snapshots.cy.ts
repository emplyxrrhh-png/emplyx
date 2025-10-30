import * as general from "../../../actions/general"
import * as screen from "../../../actions/screen"
import * as section from "../../../actions/section"
import * as spaces from "../../../actions/spaces"
import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"
import * as logoutHelper from "../../../helpers/logoutHelper"
import * as viewportHelper from "../../../helpers/viewportHelper"
import { CSS_SELECTORS_DASHBOARD } from "../../../selectors/dashboard"
import { CSS_SELECTORS_EMPLOYEE_MAINTENANCE } from "../../../selectors/functional/employeeMaintenance"
import { CSS_SELECTORS_MENU } from "../../../selectors/menu"
import { CSS_SELECTORS_USER_PROFILE_CARD } from "../../../selectors/userProfileCard"

export function normalizeHtml(html: string): string {
    return html
        .replace(/\s+/g, " ") // Reduce espacios múltiples a un solo espacio
        .replace(/id=".*?"/g, "") //Elimina IDs dinámicos
        .replace(/data-cy=".*?"/g, "") // Elimina atributos 'data-cy' usados en pruebas
        .replace(/data-testid=".*?"/g, "") // Elimina atributos 'data-testid' usados en pruebas
        .replace(/unit-test-id=".*?"/g, "") // Elimina atributos 'unit-test-id'
        .replace(/<!--.*?-->/g, "") //Elimina comentarios HTML
        .trim()
}

/*cy.get(CSS_SELECTORS_USER_PROFILE_CARD.ProfileCardWidget).then(($element) => {
                    const elementHtml = $element[0].outerHTML
                    cy.writeFile("cypress/fixtures/employeeProfileCard.html", elementHtml)
                })*/

const FIXTURES_SCREENSHOTS_FOLDER = "files/dom/"
const levelMenu: string[] = [CSS_SELECTORS_MENU.ListGroupPersonalManagement, CSS_SELECTORS_MENU.MenuOptionPersonalMaintenance]
describe("DOM Snapshot - Check different DOM snapshot should match the baseline for valid data: ", baseHelper.retriesDefault, () => {
    before(() => {
        spaces.loadCurrentUserSpace(spaces.getHRSpaceId(), "DEM1", Cypress.env("applicationUserId"))
        cy.sqlExecuteFile("cypress/fixtures/sql/automation/functional/deleteNews.sql")
        loginHelper.login(Cypress.env("applicationUserId"), Cypress.env("applicationUserPassword"))
    })

    after(() => {
        logoutHelper.logout()
    })

    viewportHelper.ViewportsDesktop.forEach((viewport: Cypress.Viewport, preset: string) => {
        context(`Desktop behavior for the device ${preset} `, viewport, () => {
            const componentName = "employeeProfileCard"
            before(() => {
                cy.visit(Cypress.config("baseUrl") + "/pop2/dashboard")
                general.clickOnElement(CSS_SELECTORS_DASHBOARD.AvatarUserName)
                general.clickOnElement(CSS_SELECTORS_USER_PROFILE_CARD.ProfileButton)
                general.waitElementToBeVisible(CSS_SELECTORS_USER_PROFILE_CARD.PhotoField)
                general.waitElementToBeVisible(CSS_SELECTORS_USER_PROFILE_CARD.EmailField)
                general.waitElementToBeVisible(CSS_SELECTORS_USER_PROFILE_CARD.NameField)
                cy.wait(2000)
            })

            it("Should 'Profile card DOM snapshot' should match withe the baseline", () => {
                const domPathFile = `${FIXTURES_SCREENSHOTS_FOLDER}employeeProfileCard.html`
                cy.fixture(domPathFile).then((savedHtml) => {
                    cy.get(CSS_SELECTORS_USER_PROFILE_CARD.ProfileCardWidget).then(($element) => {
                        const currentHtml = $element[0].outerHTML
                        const normalizedCurrentHtml = normalizeHtml(currentHtml)
                        const normalizedSavedHtml = normalizeHtml(savedHtml)
                        expect(normalizedCurrentHtml).to.equal(normalizedSavedHtml)
                    })
                })
            })
        })
    })

    viewportHelper.ViewportsDesktop.forEach((viewport: Cypress.Viewport, preset: string) => {
        context(`Desktop behavior for the device ${preset} `, viewport, () => {
            before(() => {
                viewportHelper.visitAsDevice(Cypress.config("baseUrl") + "/pop2/dashboard", preset)
                general.accessMenuOption(levelMenu)
                general.waitLoadScreenRuntimeWithList()
                cy.wait(2000)
            })

            it("Should 'Employee list DOM snapshot' should match withe the baseline", () => {
                const domPathFile = `${FIXTURES_SCREENSHOTS_FOLDER}employeeListContainer.html`
                cy.fixture(domPathFile).then((savedHtml) => {
                    cy.get(CSS_SELECTORS_EMPLOYEE_MAINTENANCE.ListEmployeeMaintenanceRecord)
                        .should("be.visible")
                        .eq(0)
                        .then(($element) => {
                            const currentHtml = $element[0].outerHTML
                            const normalizedCurrentHtml = normalizeHtml(currentHtml)
                            const normalizedSavedHtml = normalizeHtml(savedHtml)
                            expect(normalizedCurrentHtml).to.equal(normalizedSavedHtml)
                        })
                })
            })

            it("Should 'Section basic personal data DOM snapshot' should match withe the baseline", () => {
                screen.selectElementOnListByPosition(CSS_SELECTORS_EMPLOYEE_MAINTENANCE.ListEmployeeMaintenanceRecord, 0)
                section.expandMoreDetails(CSS_SELECTORS_EMPLOYEE_MAINTENANCE.SectionBasicInformationContainer)
                const domPathFile = `${FIXTURES_SCREENSHOTS_FOLDER}basicPersonInformation.html`
                cy.fixture(domPathFile).then((savedHtml) => {
                    cy.get(CSS_SELECTORS_EMPLOYEE_MAINTENANCE.SectionBasicInformationContainer).then(($element) => {
                        const currentHtml = $element[0].outerHTML
                        const normalizedCurrentHtml = normalizeHtml(currentHtml)
                        const normalizedSavedHtml = normalizeHtml(savedHtml)
                        expect(normalizedCurrentHtml).to.equal(normalizedSavedHtml)
                    })
                })
            })
        })
    })

    viewportHelper.ViewportsDesktop.forEach((viewport: Cypress.Viewport, preset: string) => {
        context(`Desktop behavior for the device ${preset} `, viewport, () => {
            before(() => {
                cy.sqlExecuteFile("cypress/fixtures/sql/automation/functional/deleteNews.sql")
                viewportHelper.visitAsDevice(Cypress.config("baseUrl") + "/pop2/dashboard", preset)
                general.waitToFinishLoading()
                cy.wait(6000)
            })

            it("Should 'Widgets dashboard RRHH DOM snapshot' should match withe the baseline", () => {
                const domPathFile = `${FIXTURES_SCREENSHOTS_FOLDER}dashboardWidgetsInfo.html`
                cy.fixture(domPathFile).then((savedHtml) => {
                    cy.get("[data-cy='app_route']").then(($element) => {
                        const currentHtml = $element[0].outerHTML
                        const normalizedCurrentHtml = normalizeHtml(currentHtml)
                        const normalizedSavedHtml = normalizeHtml(savedHtml)
                        expect(normalizedCurrentHtml).to.equal(normalizedSavedHtml)
                    })
                })
            })
        })
    })
})
