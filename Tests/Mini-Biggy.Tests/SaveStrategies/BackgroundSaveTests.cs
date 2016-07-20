﻿using System;
using System.Threading;
using MiniBiggy.SaveStrategies;
using MiniBiggy.Util;
using NUnit.Framework;

namespace MiniBiggy.Tests.SaveStrategies {
    public class BackgroundSaveTests {

        [Test]
        public void Calling_save_marks_as_dirty() {
            var strategy = new BackgroundSave(TimeSpan.FromSeconds(10));
            strategy.ShouldSaveNow();
            Assert.IsTrue(strategy.IsDirty);
        }

        [Test]
        public void Should_notify_save_when_dirty() {
            var saveCalled = false;
            using (var sem = new Semaphore(0, 1)) {
                var strategy = new BackgroundSave(TimeSpan.FromMilliseconds(100));
                strategy.NotifyUnsolicitedSave += (sender, args) => {
                    saveCalled = true;
                    sem.Release();
                };
                strategy.ShouldSaveNow();
                TimeMachine.UnblockOneOrMoreDelays();
                sem.WaitOne(1000);
                Assert.IsTrue(saveCalled);
            }
        }

        [Test]
        public void Should_not_notify_save_when_not_dirty() {
            using (var sem = new Semaphore(0, 1)) {
                var saveCalled = false;
                var strategy = new BackgroundSave(TimeSpan.FromSeconds(10));
                strategy.NotifyUnsolicitedSave += (sender, args) => {
                    saveCalled = true;
                    sem.Release();
                };
                TimeMachine.UnblockOneOrMoreDelays();
                sem.WaitOne(100);
                Assert.False(saveCalled);
            }
        }
    }
}