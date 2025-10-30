// NOTE object below must be a valid JSON
window.VTPortal = $.extend(true, window.VTPortal, {
    "config": {
        "layoutSet": "navbar",
        "navigation": [
            {
                "title": "",
                "onExecute": "#home",
                "icon": "home"
            },
            {
                "title": "",
                "onExecute": "#alerts",
                "icon": "alerts",
                "badge": "0"
            },
            //{
            //  "title": "",
            //  "onExecute": "#punches",
            //  "icon": "punches"
            //},
            {
                "title": "",
                "onExecute": "#myteam",
                "icon": "myteam",
                "badge": "0"
            }
        ],
        "supervisorNavigation": [
            {
                "title": "",
                "onExecute": "#myTeamEmployees",
                "icon": "myteam"
            },
            {
                "title": "",
                "onExecute": "#myTeamAlerts",
                "icon": "alerts",
                "badge": "0"
            },
            {
                "title": "",
                "onExecute": "#myTeamRequests",
                "icon": "myrequests",
                "badge": "0"
            },
            {
                "title": "",
                "onExecute": "#myTeamDailyRecords",
                "icon": "myrequests",
                "badge": "0"
            }
        ],
        "navigationEmployee": [
            {
                "title": "",
                "onExecute": "#home",
                "icon": "home"
            },
            {
                "title": "",
                "onExecute": "#alerts",
                "icon": "alerts",
                "badge": "0"
            },
            //{
            //    "title": "",
            //    "onExecute": "#punches",
            //    "icon": "punches"
            //}
        ],
        "impersonatedEmployee": [
            {
                "title": "",
                "onExecute": "#home",
                "icon": "home"
            },
            {
                "title": "",
                "onExecute": "#alerts",
                "icon": "alerts",
                "badge": "0"
            },
            {
                "title": "",
                "onExecute": "#home/3",
                "icon": "logout",
            }
        ]
    }
});