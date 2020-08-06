[Go Up](./)

#Jira Steps
* Newly-created tickets should be created as "Backlog" tickets, in the Backlog section of Jira.
	* Epic: Assign to Backlog epic, as this is not done automatically.
	* Issue Type:
		* Bug: Regarding the fixing of a defect.
		* Feature: Work that expands the functionality of the project.
		* Infrastructure: Related to the CI pipeline, devops, or organization tools.
		* Task: Miscellaneous task that does not directly alter the behavior of the project.
		* Epic: Major milestone under which many other tickets can be filed, culminating in a release.
	* Summary: Brief tagline for the ticket.
	* Description: Fully describe the task to be accomplished, in detail.
		* Should clearly dictate the criteria for task completion.
	* Tickets are automatically assigned an ID, in the form "MMO-??".
* During epic planning, an epic will be created for the upcoming release, and several tickets will be created, or promoted from the Backlog to the new epic.
* A developer can claim any ticket that is part of the current epic, or a bugfix ticket.
	* Move the ticket to the "In Progress" category on the Kanban board.
	* If the ticket relates to a code change:
	* Create a feature branch with the same ID as the ticket. (E.G. MMO-12)
	* Develop using this feature branch.
		* Force-pushes and history rewriting are permitted for feature branches, but take care to communicate if multiple developers are cooperating on one branch.
	* When the code change is ready to be submitted, create a pull request for it.
		* Title: Same as ticket ID.
			* Github mangles capitalization when the form is initially filled out. Put it back to the MMO-?? format.
		* First comment/PR description:
			* Describe what the change accomplishes, and how.
			* Provide a link to the Jira ticket.
				* Get the URL while looking at the "Issues" section on Jira, not the "Board" section.
		* Reviewers: Add all other team members, and tech leads.
		* Assignees: Assign yourself.
	* Move the Jira ticket to the "Review" category on the Kanban Board.
	* Discuss the changes with other reviewers as appropriate.
		* This will most likely require you to make additional pushes.
	* The PR can be merged once all reviewers have approved it.
		* Use the "Squash and Merge" method.
		* Squash commit should be a brief description of the change, and a link to the Jira ticket as in the PR description.
		* Once merged, Github will automatically delete the remote branch. It should be deleted locally as well:
			* `git checkout master; git branch -d <branch name>`
	* Move the Jira ticket to the "Done" category on the Kanban Board.
	* If the ticket does not relate to a code change, the ticket should still go through each category, including review.
		* Discuss with your team what the review process for that specific issue should look like.
* When all issues under a particular epic have been completed, a release can be made, and epic planning is conducted again.