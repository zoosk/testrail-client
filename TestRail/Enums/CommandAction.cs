using EnumStringValues;

namespace TestRail.Enums
{
    /// <summary>Command actions available.</summary>
    public enum CommandAction
    {
        /// <summary>Used to access an individual case.</summary>
        [StringValue("case")]
        Case,

        /// <summary>Used to access a set of cases.</summary>
        [StringValue("Cases")]
        Cases,

        /// <summary>Used to access the different case types.</summary>
        [StringValue("case_types")]
        CaseTypes,

        /// <summary>Used to access the different case fields.</summary>
        [StringValue("case_fields")]
        CaseFields,

        /// <summary>Used to access an individual milestone.</summary>
        [StringValue("milestone")]
        Milestone,

        /// <summary>Used to access a set of milestones.</summary>
        [StringValue("milestones")]
        Milestones,

        /// <summary>Used to access an individual plan.</summary>
        [StringValue("plan")]
        Plan,

        /// <summary>Used to access a set of plans.</summary>
        [StringValue("plans")]
        Plans,

        /// <summary>Used to access an individual plan entry.</summary>
        [StringValue("plan_entry")]
        PlanEntry,

        /// <summary>Used to access an individual project.</summary>
        [StringValue("project")]
        Project,

        /// <summary>Used to access a set of results.</summary>
        [StringValue("results")]
        Results,

        /// <summary>Used to access the results for an individual case.</summary>
        [StringValue("results_for_case")]
        ResultsForCase,

        /// <summary>Used to access the test results for a run.</summary>
        [StringValue("results_for_run")]
        ResultsForRun,

        /// <summary>Used to access an individual run.</summary>
        [StringValue("run")]
        Run,

        /// <summary>Used to access a set of runs.</summary>
        [StringValue("runs")]
        Runs,

        /// <summary>Used to access an individual section.</summary>
        [StringValue("section")]
        Section,

        /// <summary>Used to access a set of sections.</summary>
        [StringValue("sections")]
        Sections,

        /// <summary>Used to access an individual suite.</summary>
        [StringValue("suite")]
        Suite,

        /// <summary>Used to access a set of suites.</summary>
        [StringValue("suites")]
        Suites,

        /// <summary>Used to access an individual test.</summary>
        [StringValue("test")]
        Test,

        /// <summary>Used to access a set of tests.</summary>
        [StringValue("tests")]
        Tests,

        /// <summary>Used to access an individual user.</summary>
        [StringValue("user")]
        User,

        /// <summary>Used to access a set of users.</summary>
        [StringValue("users")]
        Users
    }
}
