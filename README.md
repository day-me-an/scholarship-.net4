scholarship-.net4
=================

This is a partially rewritten version of the [original C# scholarship program](https://github.com/day-me-an/scholarship-program)
that uses the .net 4.0 [Task Parallel Library (TPL)](http://en.wikipedia.org/wiki/Parallel_Extensions#Task_Parallel_Library) to radically simplify the parallel game player.

Key features:
* Self-documenting coding style.
* [Extract till you Drop](https://sites.google.com/site/unclebobconsultingllc/one-thing-extract-till-you-drop).
* Moved the old `GenerateStartingPositions()` to `PositionGenerator` containing 5 extracted methods.
* Use of TPL's `Parallel.ForEach<TSource>(IEnumerable<TSource>, Action<TSource>)` to simplify the old multi-threaded code.
* `Game.Play()` now returns an immutable object containing the outcome rather than `Game` having public properties that are dependent on `Play()` being called.
 * Same for `GamePlayer` (previously called `ParallelGame`).
* Other minor coding style changes.
