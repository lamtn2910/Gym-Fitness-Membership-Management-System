##1. Technology and setup
The application is written in C# and targets .NET 10 (net10.0). It runs in the console and stores its data in JSON files under the data/ directory, allowing records to be reloaded when the application starts again.

To run the application, install the .NET 10 SDK, open a terminal in the directory containing OOP_project.csproj, and execute:
dotnet run
The program opens an administrator menu that provides access to member management, trainer management, fitness classes, booking operations, attendance tracking, and reports.
##2. System Features and Implementation
The following sections explain the five functional requirements implemented in the system. Each section identifies the relevant classes and methods, describes how the feature works, and explains the design decisions and any remaining limitations.
###2.1. Member, MembershipPlan, Trainer, and FitnessClass Management
Classes and methods: Person, Member, MembershipPlan, BasicPlan, PremiumPlan, Trainer, FitnessClass, Admin.ManageMembers(), Admin.ManageTrainers(), Admin.ManageFitnessClasses(), Admin.TryCreateClass(), Admin.TryAssignTrainer(), Admin.TryUpdateClassSchedule(), Admin.LoadData(), and Admin.SaveData().

The system models a gym through separate classes instead of placing all information in the administrator menu. Member and Trainer inherit shared identity and contact information from the abstract Person class. A Member stores the selected MembershipPlan, join date, expiry date, and booking counter. A Trainer stores a Specialization, while a FitnessClass contains its class name, scheduled start time, duration, capacity, and assigned trainer. BasicPlan and PremiumPlan represent the two membership tiers. The property setters in Person, Trainer, and FitnessClass also validate data such as nonempty names, email format, specialization, and positive class capacity.

Admin.ManageMembers(), Admin.ManageTrainers(), and Admin.ManageFitnessClasses() provide the console workflows for creating, viewing, editing, and deleting records. However, important scheduling decisions are delegated to methods in AdminScheduling.cs: TryCreateClass() rejects an invalid class schedule or a trainer who is already teaching an overlapping class; TryAssignTrainer() checks for trainer conflicts before replacing the class's assigned trainer; and TryUpdateClassSchedule() checks both trainer and member conflicts before changing a class's time or duration. Admin.LoadData() and Admin.SaveData() restore and persist records in JSON.

Why this design was chosen: Giving each entity its own class makes its data and validation responsibilities easier to understand and maintain. The administrator coordinates operations involving several entities, while the individual model classes represent the state of members, trainers, plans, and classes. The trainer's specialization is recorded, but the current assignment methods do not yet require it to match the class type.
###2.2. Class Booking, Capacity Limits, and Waitlist
Classes and methods: Booking, BookingStatus, FitnessClass.Capacity, FitnessClass.Roster, FitnessClass.WaitList, FitnessClass.BookClass(Member member), FitnessClass.CancelBooking(string bookingId), FitnessClass.PromoteFromWaitlist(), Admin.TryBookClass(...), and Admin.ManageBookings().

Each booking is represented by a Booking object connecting one member to one fitness class, together with a booking date and a status (Confirmed, WaitListed, Cancelled, or Attended). Admin.TryBookClass() first validates the request and then calls FitnessClass.BookClass(). The class compares the number of confirmed participants in _roster with Capacity. If space is available, it creates a Confirmed booking and increments the member's booking count. If the class is full, it creates a WaitListed booking and appends it to _waitList without consuming a confirmed-booking slot.

FitnessClass.CancelBooking() removes an active booking from its roster or waitlist and updates its status. Cancelling a confirmed booking frees a seat and immediately calls PromoteFromWaitlist(). Increasing capacity through Admin.ManageFitnessClasses() also triggers promotion while seats are available. The waitlist uses a queue to preserve request order, with eligibility checks applied before each promotion. The detailed eligibility-aware FIFO procedure is explained in Section 5.

Why this design was chosen: FitnessClass owns the roster and waitlist because capacity is a property of an individual class. Keeping seat allocation, cancellation, and promotion in the same class prevents the console interface from independently changing those collections and makes the booking lifecycle easier to maintain.
###2.3. Membership-Tier Booking Rules
Classes and methods: MembershipPlan.CanBookClass(int currentBookingCount), MembershipPlan.MaxBookingsPerMonth, BasicPlan.MaxBookingsPerMonth, PremiumPlan.MaxBookingsPerMonth, Member.CanBookMore(), Member.IncrementBookingCount(), Member.DecrementBookingCount(), Admin.ValidateBooking(...), FitnessClass.BookClass(Member member), FitnessClass.PromoteFromWaitlist(), and Admin.CanPromoteBooking(...).

The abstract MembershipPlan class defines the common rule CanBookClass(), which compares the member's booking counter with the current plan's maximum. BasicPlan overrides MaxBookingsPerMonth with 5, while PremiumPlan returns int.MaxValue. When a member requests a class, Member.CanBookMore() delegates the decision to the actual plan object in CurrentPlan. Both Admin.ValidateBooking() and FitnessClass.BookClass() use this result to reject an ineligible booking; promotion from the waitlist checks eligibility again because the member's situation may have changed while waiting.

The booking counter increases when a booking becomes Confirmed and decreases when a confirmed booking is cancelled. Joining a waitlist does not increase the counter, although a member who has already reached the plan limit is currently rejected even from joining that waitlist. Because the limit is implemented through the plan hierarchy, the booking code does not need a separate branch for each membership tier.

Why this design was chosen: Membership benefits can change without rewriting the booking workflow. The current implementation does not automatically reset CurrentMonthlyBookings each calendar month, and it does not reject a booking solely because PlanExpiryDate has passed. Therefore, the code currently enforces the tier limit against its stored counter rather than a complete calendar-month and expiry-aware policy.
###2.4. Attendance Tracking and Activity Reports
Classes and methods: Admin.MarkAttendance(), FitnessClass.GetConfirmedAttendees(), FitnessClass.MarkAttendance(string memberId), Booking.MarkAttended(), BookingStatus.Attended, Admin.GenerateReports(), and ActivityReportService.GenerateMemberReport(Member member, List<Booking> bookings).

The attendance workflow begins in Admin.MarkAttendance(), where the administrator selects a class and a member. The menu looks up that member's booking in the class roster, rejects members who are not on the confirmed roster, and prevents marking a booking that has already been attended. It then calls FitnessClass.MarkAttendance(), which finds the booking and invokes Booking.MarkAttended() to change its status to Attended. The administrator saves the updated data to JSON through SaveData().

For reporting, Admin.GenerateReports() filters _bookingHistory by member and passes the matching records to ActivityReportService.GenerateMemberReport(). The service produces a text report showing member contact details, membership plan and expiry date, total bookings, counts of attended, confirmed, waitlisted, and cancelled bookings, and a list of classes with their scheduled times and statuses. The menu can display a report for one member, reports for all members, or a simple system-wide summary. Reports are printed to the console rather than exported as separate files.

Why this design was chosen: Attendance changes are performed through the booking and class objects, while report formatting is kept in ActivityReportService. This separation prevents reporting code from being mixed into the attendance logic and allows the same saved booking statuses to support both individual and overall summaries.
###2.5. Membership Renewal, Upgrade, and Downgrade
Classes and methods: Admin.ManageMembers() (menu option 4), Member.ChangePlan(MembershipPlan newPlan), Member.CurrentPlan, Member.PlanExpiryDate, Member.CanBookMore(), BasicPlan, PremiumPlan, and Admin.SaveData().

The implemented plan-change workflow is found in option 4 of Admin.ManageMembers(). The administrator selects a member and chooses BasicPlan or PremiumPlan. The menu avoids changing to the same plan again; otherwise, it calls Member.ChangePlan(), which replaces CurrentPlan and sets PlanExpiryDate to one month after the change date. Admin.SaveData() then persists the new plan name and expiry date. Choosing Premium when the member currently has Basic is an upgrade, and choosing Basic when the member currently has Premium is a downgrade.

The current plan-change policy is immediate activation with a new one-month expiry date. The existing CurrentMonthlyBookings value is preserved, and future eligibility checks use the new plan's limit. For example, a downgrade to Basic may prevent new confirmations while the stored counter is already at or above five; the change does not automatically cancel existing bookings. This is the behavior actually implemented in Member.ChangePlan() and Member.CanBookMore().

Why this design was chosen and what remains incomplete: A plan change only replaces the MembershipPlan object, so the shared booking logic automatically follows the new tier. However, the program does not currently calculate prorated charges or refunds, and it does not provide a dedicated renewal command that extends an existing plan without switching tiers. The expiry date is saved but is not enforced during booking. Consequently, upgrade and downgrade are implemented under the rule described above, while a complete renewal and payment policy remains unfinished; this requirement should not be presented as fully complete.
##3. Project structure
The project is organized into the following main source files:
- Main.cs: Application entry point and console menu.
- Admin.cs: Administrator operations and JSON persistence.
- AdminScheduling.cs: Booking and scheduling validation across classes.
- Person.cs: Shared base class for people.
- Member.cs: Member details and membership management.
- Trainer.cs: Trainer information.
- MembershipPlan.cs: Abstract class defining common membership benefits.
- BasicPlan.cs: Rules and benefits for the Basic membership tier.
- PremiumPlan.cs: Rules and benefits for the Premium membership tier.
- FitnessClass.cs: Fitness class management, confirmed roster, waitlist, and booking lifecycle.
- Booking.cs: Booking records and related information.
- BookingStatus.cs: Booking status definitions.
- ScheduleRules.cs: Schedule-overlap checking.
- ActivityReportService.cs: Individual member activity reports.
- ConsoleHelper.cs: Input validation helpers for the console interface.
- OOP_project.csproj: .NET project configuration.
- data/: JSON files used for persistent storage, including:
  - members.json: Member data.
  - trainers.json: Trainer data.
  - fitnessClasses.json: Fitness class data.
  - bookings.json: Booking data.
The booking logic is mainly located in FitnessClass.cs, while AdminScheduling.cs coordinates rules that depend on information from several classes. Membership benefits are implemented through the MembershipPlan hierarchy, and the activity report logic is separated into ActivityReportService.cs.
##4. Object-Oriented design principles
###4.1. Encapsulation in booking management
Classes and methods: FitnessClass.BookClass(Member member), FitnessClass.CancelBooking(string bookingId), FitnessClass.PromoteFromWaitlist(), Member.CanBookMore(), Member.IncrementBookingCount(), Member.DecrementBookingCount(), Admin.TryBookClass(...), Admin.ValidateBooking(...), and Admin.RegisterClass(...).

Encapsulation is applied primarily in the FitnessClass class. It stores confirmed bookings in the private field _roster (List<Booking>) and waiting bookings in the private field _waitList (Queue<Booking>). Other classes can inspect them using the Roster and WaitList properties, but the intended way to modify booking state is through methods provided by FitnessClass. This keeps the class responsible for updating its own booking collections.

The FitnessClass.BookClass() method controls the booking process rather than allowing the console menu to add a booking directly to a collection. It checks whether the class has already started, whether the member has an active booking for the same class, and whether Member.CanBookMore() allows another confirmed booking. It also invokes the registered BookingValidation callback for rules that depend on other classes. If the request is valid and a seat is available, the new booking is confirmed and added to _roster, and the member's booking counter is increased. If the class is full, the booking is marked WaitListed and placed in _waitList without increasing that counter.

The FitnessClass.CancelBooking() method performs the corresponding state changes when a member cancels. It updates the booking status, removes the active booking from the appropriate collection, and reduces the confirmed-booking counter when necessary. If a confirmed seat becomes available, it calls PromoteFromWaitlist() to handle the next eligible member. As a result, related updates take place together instead of being scattered across different menu commands.

Some rules require information about all classes rather than one FitnessClass. Those checks are implemented in Admin.ValidateBooking() in AdminScheduling.cs. The Admin.RegisterClass() method connects them to the class through BookingValidation, while Admin.TryBookClass() provides the administrator's validated booking entry point. This design keeps the responsibility for changing a booking inside FitnessClass and lets Admin supply cross-class rules, such as checking schedule conflicts.

Why this principle is appropriate: Booking a class changes several related values: the roster, the waitlist, booking status, and the member's booking count. Encapsulation helps prevent inconsistent states because these updates are handled by controlled methods instead of being performed independently by the user interface.
###4.2. Polymorphism through MembershipPlan
Classes and methods: MembershipPlan, MembershipPlan.MaxBookingsPerMonth, MembershipPlan.CanBookClass(int currentBookingCount), BasicPlan.MaxBookingsPerMonth, PremiumPlan.MaxBookingsPerMonth, Member.CurrentPlan, Member.CanBookMore(), and Member.ChangePlan(MembershipPlan newPlan).

Polymorphism is demonstrated through the abstract MembershipPlan class and its concrete subclasses, BasicPlan and PremiumPlan. Both plans share the same interface for checking booking eligibility, but each plan supplies a different value for the overridden MaxBookingsPerMonth property. The Basic plan allows five confirmed bookings under the implemented counter rule, while the Premium plan uses int.MaxValue as a practically unlimited maximum.

A member's CurrentPlan property has the base type MembershipPlan, so the rest of the program does not need to know whether the member currently uses BasicPlan or PremiumPlan. When Member.CanBookMore() calls CurrentPlan.CanBookClass(CurrentMonthlyBookings), the inherited method uses the booking limit of the actual plan object. For example, a member with five counted bookings cannot add another confirmed booking under Basic, but the same count remains within the Premium limit.

The Member.ChangePlan() method replaces the current plan object. Subsequent calls to Member.CanBookMore() then follow the new plan's benefits without changing the booking algorithm in FitnessClass.BookClass(). In this implementation, the polymorphic behavior comes specifically from the overridden MaxBookingsPerMonth property used by the shared CanBookClass() method; BasicPlan and PremiumPlan do not need separate implementations of that method.

Why this principle is appropriate: Different membership tiers have different benefits, but the class-booking process should remain the same. Using the abstract MembershipPlan type makes it possible to add or modify membership tiers without filling the booking logic with checks for specific plan names.
###4.3. Composition in FitnessClass
Classes and methods: FitnessClass, its private _roster and _waitList fields, the Roster and WaitList properties, and the methods BookClass(), CancelBooking(), PromoteFromWaitlist(), CancelAllBookings(), and RestoreBooking(Booking booking).

Composition appears in the way each FitnessClass contains and manages its own booking collections. The private _roster field is a List<Booking> containing confirmed participants, while _waitList is a Queue<Booking> containing members waiting for a seat. These two collections are created as part of the FitnessClass object and are maintained through its methods. The console interface does not maintain separate copies of a class roster or waitlist.

When a member books a class, BookClass() adds the booking to the appropriate collection. When a booking is cancelled, CancelBooking() updates the collections, and PromoteFromWaitlist() may move an eligible waiting booking into the confirmed roster. When previously saved booking records are loaded, RestoreBooking() reconstructs the class's collections from those records. The FitnessClass object therefore coordinates the data that belongs to its own booking process.

Why this principle is appropriate: A roster and a waitlist are meaningful only in the context of a particular class. Placing both collections inside FitnessClass makes it easier to enforce capacity limits and maintain correct waiting order. More precisely, the collections are owned by FitnessClass; individual Booking records are also referenced by the system's booking history and are not exclusively owned by the class.
##5. FIFO waitlist promotion algorithm
Classes and methods: FitnessClass.BookClass(), FitnessClass.CancelBooking(), FitnessClass.PromoteFromWaitlist(), and Admin.CanPromoteBooking().

The waitlist is implemented as a Queue<Booking>, following the First In, First Out (FIFO) principle. When a member books a class that has reached its capacity, FitnessClass.BookClass() sets the new booking status to WaitListed and calls _waitList.Enqueue(booking). The new member joins the end of the queue, so earlier requests remain ahead of later ones.

When a confirmed booking is cancelled, FitnessClass.CancelBooking() releases the seat and calls PromoteFromWaitlist(). Promotion can also be requested through the administrator's commands, including after an increase in class capacity. The method does not promote anyone if the class has already started, if no seat is available, or if the waitlist is empty.

To choose a member, PromoteFromWaitlist() records the current queue length and examines at most that many entries in order. It removes each entry from the front using Dequeue() and checks whether the booking still has WaitListed status, the member is allowed to make another booking, and the member does not already have a confirmed booking in the same class. It also calls the registered PromotionValidation callback, if available. In this project, Admin.CanPromoteBooking() supplies further checks, including whether the member and class still exist and whether the member has a conflicting booking elsewhere.

The first member who passes the checks is selected for promotion. Other entries that are still waitlisted are added back to the queue in their original relative order. A member who is currently ineligible is therefore skipped for this promotion attempt but does not automatically lose their place on the waitlist. If an eligible member is found, the method changes their booking status to Confirmed, adds their booking to _roster, increments their booking counter, and returns true. Otherwise, it returns false without filling the seat.

The behavior is best described as eligibility-aware FIFO. The algorithm respects arrival order among members who can be promoted at the time of the check, while preserving the relative order of those who remain waiting. One call to PromoteFromWaitlist() scans at most n entries, so its time complexity is O(n), with O(1) additional space beyond the existing queue. Each successful call promotes at most one member.
##6. Data persistence
The system uses JSON files to retain its data across application sessions. Admin.LoadData() reads member records from members.json, trainer records from trainers.json, class information from fitnessClasses.json, and booking records from bookings.json. During loading, FitnessClass.RestoreBooking() reconstructs confirmed rosters and waitlists from the saved bookings. Waiting entries are restored in the order in which they appear in bookings.json. The Admin.SaveData() method saves changes and is also called when the application exits.
##7. Current implementation notes and limitations
The project supports changing a member's plan through Member.ChangePlan(), which replaces the plan object and sets a new expiry date. However, the current implementation does not calculate prorated payments when upgrading or downgrading, and it does not provide a separate full renewal workflow.

The CurrentMonthlyBookings field tracks bookings through increment and decrement operations. Although ResetMonthlyBookings() exists, the program does not automatically reset this counter at the start of each calendar month. When loading saved data, the counter is reconstructed from relevant restored bookings without filtering them to the current month. In addition, the plan expiry date is stored but is not currently used to reject bookings after expiration.

Trainer information includes a specialization field. The current class-creation and trainer-assignment methods check scheduling conflicts, but they do not yet enforce a match between the trainer's specialization and the class type. These limitations are noted to distinguish implemented behavior from possible future improvements.
