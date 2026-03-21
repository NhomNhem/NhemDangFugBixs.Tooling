# CI Performance Regression Checks

This project uses automated CI checks to detect performance regressions in build, test, and code generation times. The workflow is implemented in `.github/workflows/ci-validation.yml` and leverages:

- **Incremental change detection**: Only changed projects are rebuilt and tested.
- **Selective test execution**: Only affected test suites are run.
- **NuGet caching**: Dependencies are restored from cache for faster runs.
- **BenchmarkDotNet integration**: Benchmarks can be run locally or in CI to monitor generator/analyzer performance.
- **Performance baseline documentation**: See `docs/CI-BENCHMARK-REPORT.md` and `docs/CI-PERFORMANCE-SUMMARY.md` for historical metrics and improvement tracking.

## How it works

1. **Detect Changes**: The `detect-changes` job identifies which projects have changed using `git diff`.
2. **Build & Test**: Only changed projects are built and only relevant tests are run. If shared code changes, a full build/test is triggered.
3. **Performance Monitoring**: Execution times are measured and compared to baselines. Significant regressions can be flagged (see below).
4. **Local Benchmarking**: Run `scripts/ci-benchmark.ps1` to simulate CI scenarios and verify local performance before pushing.

## Failing on Regressions

- The workflow can be extended to compare build/test times to historical baselines (see `docs/CI-BENCHMARK-REPORT.md`).
- If a step exceeds a threshold (e.g., build > 30s, test > 20s), the job can fail with a message.
- To add this, insert a shell step after build/test:

```yaml
- name: Check build time
  run: |
    if [ $BUILD_TIME -gt 30 ]; then
      echo "Build time regression detected!"
      exit 1
    fi
```

## BenchmarkDotNet Integration

- Benchmarks are defined in `Source~/DangFugBixs.Generators~/DangFugBixs.Generators.Benchmarks`.
- To run locally:
  ```sh
  dotnet run -c Release -p Source~/DangFugBixs.Generators~/DangFugBixs.Generators.Benchmarks/DangFugBixs.Generators.Benchmarks.csproj
  ```
- Results can be compared to previous runs to detect regressions.

## References

- [docs/CI-BENCHMARK-REPORT.md](docs/CI-BENCHMARK-REPORT.md)
- [docs/CI-PERFORMANCE-SUMMARY.md](docs/CI-PERFORMANCE-SUMMARY.md)
- [scripts/ci-benchmark.ps1](scripts/ci-benchmark.ps1)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)

---

**Status:** CI performance regression checks are implemented and documented. See workflow and docs for details.
